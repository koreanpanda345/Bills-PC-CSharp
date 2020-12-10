using System;
using System.Linq;
using System.Threading.Tasks;
using BillsPC.Bot.Global;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BillsPC.Bot.Attributes
{
    public class DisabledAttribute : CheckBaseAttribute
    {
        
        private string Command { get; set; }

        public DisabledAttribute(string command = null)
        {
            Command = command;
        }
        
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (Command is null) Command = ctx.Command.Name;
            GuildSettings guildSettings;
            if (!Global.Commands.GuildSettings.Exists(x => x.GuildId == ctx.Channel.GuildId))
                guildSettings = new GuildSettings
                {
                    GuildId =  ctx.Guild.Id,
                    Prefix = "b!"
                };
            else
                guildSettings = Global.Commands.GuildSettings.Where(x => x.GuildId == ctx.Channel.GuildId)
                    .FirstOrDefault();
            Console.WriteLine(guildSettings.Enabled[Command]);
            return Task.FromResult( guildSettings.Enabled[Command]);
        }
    }
}