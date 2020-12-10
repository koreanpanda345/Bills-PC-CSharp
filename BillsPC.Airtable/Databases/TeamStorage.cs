using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;
using BillsPC.Airtable.Models;

namespace BillsPC.Airtable.Databases
{
    /// <summary>
    /// Inherits DatabaseBase
    /// </summary>
    public class TeamStorage : DatabaseBase
    {
        /// <summary>
        /// Creates the DatabaseBase.
        /// </summary>
        /// <param name="apiKey">The airtable's api key</param>
        /// <param name="baseId">The airtable's base id</param>
        public TeamStorage(string apiKey, string baseId) : base(apiKey, baseId)
        {
        }
        /// <summary>
        /// Creates the User's record.
        /// </summary>
        /// <param name="id">the user's id</param>
        /// <returns>Returns Nothing, because async Task results to void.</returns>
        public async Task CreateUserRecord(ulong id) 
            => await base.CreateData("Teams", fields =>
            {
                fields.AddField("userId", id);
                return fields;
            });
        /// <summary>
        /// Inserts a new team into the user's record.
        /// </summary>
        /// <param name="id">the user's id</param>
        /// <param name="team">the new team that will be added to the record.</param>
        /// <returns>Returns Nothing, because async Task results to void.</returns>
        public async Task InsertTeam(ulong id, TeamModel team)
            => await base.InsertData("Teams", $"userId = {id}", async fields =>
            {
                var teams = await GetTeams(id);
                teams.Add(team);
                var names = "";
                var pastes = "";
                foreach (var _team in teams)
                {
                    names += $"{_team.Name}\n";
                    pastes += $"{_team.ToString()}\n\n\n\n";
                }
                fields.AddField("Names", names);
                fields.AddField("Pastes", pastes);
                return fields;
            });
        /// <summary>
        /// Gets the user's teams.
        /// </summary>
        /// <param name="id">the user's id</param>
        /// <returns>Returns the user's teams in a List<TeamModel>.</returns>
        public async Task<List<TeamModel>> GetTeams(ulong id)
            => await base.GetData<List<TeamModel>>("Teams", $"userId = {id}",(records, model) =>
            {
                var record = records.FirstOrDefault();
                var names = Convert.ToString(record.GetField("Names")).Split("\n");
                var pastes = Convert.ToString(record.GetField("Pastes")).Split("\n\n\n\n");
                for (var i = 0; i < names.Length; i++)
                {
                    model.Add(new TeamModel()
                    {
                        Name = names[i]
                    });

                    foreach (var poke in pastes[i].Split("\n\n"))
                    {
                        model[i].Pokemon.Add(new TeamPokemonModel().ConvertFrom(poke));
                    }
                }
                return model;
            });
        
        /// <summary>
        /// Gets the user's team that they requested for.
        /// </summary>
        /// <param name="id">the user's id.</param>
        /// <param name="teamId">the team's id (index) that the user requested for.</param>
        /// <returns>Returns the user's team in a TeamModel.</returns>
        public async Task<TeamModel> GetTeam(ulong id, int teamId)
            => await base.GetData<TeamModel>("Teams", $"userId = {id}", (records, model) =>
            {
                var record = records.FirstOrDefault();
                model.Name = Convert.ToString(record.GetField("Names")).Split("\n")[teamId];
                foreach (var poke in Convert.ToString(record.GetField("Pastes")).Split("\n\n\n\n")[teamId].Split("\n\n"))
                {
                    model.Pokemon.Add(new TeamPokemonModel().ConvertFrom(poke)); 
                } 
                return model;
            });
    }
}