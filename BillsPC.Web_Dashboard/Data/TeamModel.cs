using System.Collections.Generic;

namespace BillsPC.Web_Dashboard.Data
{
    public class TeamModel
    {
        public string TeamName { get; set; }
        public List<TeamPokemonModel> Pokemon { get; set; } = new List<TeamPokemonModel>();
    }

    public class TeamPokemonModel
    {
        public string Name { get; set; }
        public string Item { get; set; }
        public string Ability { get; set; }
        public string Move1 { get; set; }
        public string Move2 { get; set; }
        public string Move3 { get; set; }
        public string Move4 { get; set; }
        public int[] Ivs { get; set; }
        public int[] Evs { get; set; }
        public string Nature { get; set; }
        public string IconUrl { get; set; }
    }
}