using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillsPC.Bot.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace BillsPC.Bot.Commands
{
    public class Information : BaseCommandModule
    {
        
        [Command("latency")]
        [Aliases("ping")]
        [Description("Displays my latency")]
        [Disabled()]
        public async Task LatencyCommand(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder();
            embed.Title = "API Latency";
            if (Global.Commands.GuildSettings.Where(x => x.GuildId == ctx.Guild.Id).FirstOrDefault().VerboseMode)
            {
                embed.Description = $"Pong!\nPing: {ctx.Client.Ping}";
                embed.Color = ctx.Client.Ping <= 100 ? DiscordColor.Green :
                    ctx.Client.Ping <= 200 ? DiscordColor.Orange : DiscordColor.Red;
            }
            else
            {
                embed.Description = $"My Latency is {ctx.Client.Ping} ms!";
                embed.Color = ctx.Client.Ping <= 100 ? DiscordColor.Green :
                    ctx.Client.Ping <= 200 ? DiscordColor.Orange : DiscordColor.Red;
            }

            await ctx.RespondAsync(embed: embed.Build());
        }

        [Command("dashboard")]
        [Aliases("settings")]
        [Description("Sends you the link to the dashboard")]
        public async Task DashboardCommand(CommandContext ctx)
        {
            var dashboardUrl = "https://localhost:5001";
            await ctx.RespondAsync(dashboardUrl);
        }
        
    }
}