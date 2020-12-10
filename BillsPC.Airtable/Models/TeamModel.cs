using System.Collections.Generic;

namespace BillsPC.Airtable.Models
{
    public class TeamModel
    {
        public string Name { get; set; }
        public List<TeamPokemonModel> Pokemon { get; set; } = new List<TeamPokemonModel>();

        public override string ToString()
        {
            var paste = "";
            foreach (var pokemon in Pokemon)
                paste += $"{pokemon.ToString()}\n";
            return paste;
        }
    }
}