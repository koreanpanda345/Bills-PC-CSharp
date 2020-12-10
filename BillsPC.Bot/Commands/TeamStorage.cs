using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace BillsPC.Bot.Commands
{
    public class TeamStorage : BaseCommandModule
    {
        
        private Airtable.Databases.TeamStorage Storage { get; set; }

        public TeamStorage(IServiceProvider services)
        {
            Storage = services.GetRequiredService<Airtable.Databases.TeamStorage>();
        }
        
        [Command("getteam")]
        [Aliases("gt")]
        [Description("Gets a team from the PC")]
        public async Task GetTeam(CommandContext ctx, int teamId)
        {
            var team = await Storage.GetTeam(ctx.User.Id, teamId - 1);
            var builder = new DiscordEmbedBuilder()
            {
                Title = $"{team.Name}",
                Description = $"{team.ToString()}",
                Color = DiscordColor.Green
            };

            await ctx.RespondAsync(embed: builder.Build());
        }
    }
}