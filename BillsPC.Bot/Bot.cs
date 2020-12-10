using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillsPC.Airtable.Models;
using BillsPC.Bot.Attributes;
using BillsPC.Bot.Commands;
using BillsPC.Bot.Global;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace BillsPC.Bot
{
    public class Bot
    {
        #region Variables
        private DiscordClient Client { get; set; }
        public CommandsNextExtension CommandsNext { get; set; }
        private InteractivityExtension Interactivity { get; set; }
        private IServiceProvider Services { get; set; }
        private Airtable.Databases.TeamStorage TeamStorage { get; set; }
        private string DefaultPrefix { get; set; }
        #endregion

        #region Constructor
        public Bot(string token, string prefix, string apiKey, string baseId)
        {
            // Initialize the client object.
            Client = new DiscordClient(new DiscordConfiguration
            {
                MinimumLogLevel = LogLevel.Information,
                Token = token,
                Intents = DiscordIntents.GuildMessages | DiscordIntents.Guilds | DiscordIntents.GuildEmojis | DiscordIntents.DirectMessages
            });
            // Initialize the interactivity object.
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });
            // Initialize Serilog.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();
            // Initialize The TeamStorage Database.
            TeamStorage = new Airtable.Databases.TeamStorage(apiKey, baseId);
            // Initialize the Service Provider
            Services = new ServiceCollection()
                .AddSingleton(TeamStorage)
                .AddSingleton(Interactivity)
                .AddSingleton(Log.Logger)
                .BuildServiceProvider();
            // Default Prefix
            DefaultPrefix = prefix;
            // Initialize the CommandsNext object.
            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = false,
                Services = Services,
                PrefixResolver = CustomPrefixPrecondition, // Event for Server custom prefixes.
                IgnoreExtraArguments = true
            });
            CommandsNext.RegisterCommands<Information>(); // Registers Information module's commands.
            CommandsNext.RegisterCommands<Commands.TeamStorage>(); // Registers Team Storage commands.
            Client.ConnectAsync(); // Logs the bot into discord.
            Task.Delay(-1); // Prevents the bot from exiting after connecting.

            CommandsNext.CommandErrored += CommandsNextOnCommandErrored; // Command Errored Event.
        }
        #endregion

        #region Events
        /// <summary>
        /// Handles the prefixes. Checks to see if the message sent in the server matches with the
        /// server's prefix.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static Task<int> CustomPrefixPrecondition(DiscordMessage msg)
        {
            var guild = msg.Channel.Guild;
            string prefix;
            if (Global.Commands.GuildSettings.Exists(x => x.GuildId == msg.Channel.GuildId))
                prefix = "b!";
            else prefix = Global.Commands.GuildSettings.Where(x => x.GuildId == msg.Channel.GuildId).FirstOrDefault().Prefix;

                return msg.Content.StartsWith(prefix)
                ? Task.FromResult(prefix.Length)
                : Task.FromResult(-1);
        }
        /// <summary>
        /// Command Errored Event. This handles the events that the command returns an error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task CommandsNextOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is System.ArgumentException) // Check to see if the error is caused by the user not supplying enough arguments.
            {
                await e.Context.RespondAsync(
                    "This command requires arguments. please refer to the help command, or my dashboard, to see what arguments this command needs");
                return;
            }
            var failedChecks = ((ChecksFailedException) e.Exception).FailedChecks;
            foreach (var failedCheck in failedChecks)
            {
                
                if (failedCheck is DisabledAttribute) // Checks if the command is disabled.
                {
                    await e.Context.Channel.SendMessageAsync("This command was disabled.");
                }
                else // if the command error is something that is not handled, then we are going to handle, and send info of why.
                {
                    var builder = new DiscordEmbedBuilder()
                    {
                        Title = $"Exception: {e.Exception.GetType().ToString()}",
                        Description = e.Exception.Message,
                        Color = DiscordColor.Red
                    };
                    builder.AddField("Stack", e.Exception.StackTrace);
                    builder.AddField("Inner Exception", e.Exception.InnerException.Message);

                    await e.Context.RespondAsync(embed: builder.Build());
                }
            }
            
        }
        #endregion

        #region Dashboard Methods
        
        /// <summary>
        /// Allows the Dashboard to get Bot's Commands
        /// </summary>
        /// <returns></returns>
        public List<Command> GetCommands() => CommandsNext.RegisteredCommands.Values.ToList();
        /// <summary>
        /// Checks to see if the guild's settings exist. if not, then we will make them.
        /// Then we will put the guild's settings into a callback, so we can access the settings faster.
        /// </summary>
        /// <param name="guildId">the server's id.</param>
        /// <param name="callback">getting the guild's settings, so we can access it faster.</param>
        private void IfGuildSettingsDontExist(ulong guildId, Func<GuildSettings, Task> callback)
        {
            if(!Global.Commands.GuildSettings.Exists(x => x.GuildId == guildId))
                Global.Commands.GuildSettings.Add(new GuildSettings()
                {
                    GuildId = guildId,
                    Prefix =  DefaultPrefix
                });
            // This is basically a return statement in a way, but we are not returning anything.
            callback(Global.Commands.GuildSettings.Where(x => x.GuildId == guildId).FirstOrDefault());
        }
        /// <summary>
        /// Disables a command for a server.
        /// </summary>
        /// <param name="command">the command's name</param>
        /// <param name="guildId">the server's id</param>
        public void DisableCommand(string command, ulong guildId) =>
            IfGuildSettingsDontExist(guildId, (guildSettings) =>
            {
                guildSettings.DisableCommand(command);
                return Task.CompletedTask;
            });
        /// <summary>
        /// Enables a command for a server.
        /// </summary>
        /// <param name="command">the command's name</param>
        /// <param name="guildId">the server's id</param>
        public void EnableCommand(string command, ulong guildId) => IfGuildSettingsDontExist(guildId, (guildSettings) =>
        {
            guildSettings.EnableCommand(command);
            return Task.CompletedTask;
        });
        /// <summary>
        /// Checks if the command is enabled for a server.
        /// </summary>
        /// <param name="command">the command's name</param>
        /// <param name="guildId">the server's id</param>
        /// <returns>if the command is enabled or not.</returns>
        public bool IsCommandEnabled(string command, ulong guildId)
        {
            var enabled = false;
            IfGuildSettingsDontExist(guildId, settings =>
            {
                enabled = settings.Enabled[command];
                return Task.CompletedTask;
            });

            return enabled;
        }
        /// <summary>
        /// Gets the server's prefix.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        /// <returns>the server's prefix</returns>
        public string GetPrefix(ulong guildId)
        {
            var prefix = "";
            IfGuildSettingsDontExist(guildId, settings =>
            {
                prefix = settings.Prefix;
                return Task.CompletedTask;
            });
            return prefix;
        }
        /// <summary>
        /// Sets the server's prefix.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        /// <param name="prefix">the new prefix for the server.</param>
        public void SetPrefix(ulong guildId, string prefix) => IfGuildSettingsDontExist(guildId, settings =>
        {
            settings.Prefix = prefix;
            return Task.CompletedTask;
        });
        /// <summary>
        /// Resets the server's prefix.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        public void ResetPrefix(ulong guildId) => IfGuildSettingsDontExist(guildId, settings =>
        {
            settings.Prefix = DefaultPrefix;
            return Task.CompletedTask;
        });
        /// <summary>
        /// Enable Verbose Mode for a server.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        public void EnableVerboseMode(ulong guildId) => IfGuildSettingsDontExist(guildId, settings =>
        {
            settings.VerboseMode = true;
            return Task.CompletedTask;
        });
        /// <summary>
        /// Disables Verbose Mode for a server.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        public void DisableVerboseMode(ulong guildId) => IfGuildSettingsDontExist(guildId, settings =>
        {
            settings.VerboseMode = false;
            return Task.CompletedTask;
        });
        /// <summary>
        /// Checks if the Verbose Mode is disabled for a server.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        /// <returns>If Verbose mode is enabled or not.</returns>
        public bool GetVerboseMode(ulong guildId)
        {
            var enabled = false;
            IfGuildSettingsDontExist(guildId, settings =>
            {
                enabled = settings.VerboseMode;
                return Task.CompletedTask;
            });

            return enabled;
        }
        /// <summary>
        /// Checks if the bot is in the server.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        /// <returns>If the bot is in the server.</returns>
        public bool IsBotInServer(ulong guildId) => Client.Guilds.ContainsKey(guildId);
        /// <summary>
        /// Gets the Guild Object.
        /// </summary>
        /// <param name="guildId">the server's id</param>
        /// <returns>The Guild Object for the server.</returns>
        public DiscordGuild GetGuild(ulong guildId) => Client.Guilds[guildId];
        /// <summary>
        /// Gets the server count.
        /// </summary>
        /// <returns>The amount of servers the bot is in.</returns>
        public int GetServerCount() => Client.Guilds.Count;
        /// <summary>
        /// Gets the member count.
        /// </summary>
        /// <returns>the amount of members in all the servers combined that the bot is in.</returns>
        public int GetMemberCount()
        {
            var memberCount = 0;
            foreach (var guild in Client.Guilds)
            {
                memberCount += guild.Value.MemberCount;
            }

            return memberCount;
        }
        /// <summary>
        /// Gets the bot's latency
        /// </summary>
        /// <returns>the bot's latency</returns>
        public int GetLatency() => Client.Ping;
        /// <summary>
        /// Gets the bot's avatar.
        /// </summary>
        /// <returns>the bot's avatar url</returns>
        public string GetAvatarUrl() =>
            Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto) ?? Client.CurrentUser.DefaultAvatarUrl;
        /// <summary>
        /// Gets the user's team.
        /// </summary>
        /// <param name="userId">the user's id</param>
        /// <param name="teamId">the team's id</param>
        /// <returns>TeamModel object of the user's requested team.</returns>
        public async Task<TeamModel> GetTeam(ulong userId, int teamId) => await TeamStorage.GetTeam(userId, teamId);
        /// <summary>
        /// Gets the user's teams.
        /// </summary>
        /// <param name="userId">the user's id</param>
        /// <returns>List<TeamModel> object of all the user's teams.</returns>
        public async Task<List<TeamModel>> GetTeams(ulong userId) => await TeamStorage.GetTeams(userId);

        #endregion
    }
}