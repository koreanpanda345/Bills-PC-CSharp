using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsPC.Airtable.Models
{
    public class TeamPokemonModel
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public string Item { get; set; }
        public string Ability { get; set; }
        public string Nature { get; set; }

        public Dictionary<string, int> Ivs { get; set; } = new Dictionary<string, int>()
        {
            {"hp", 31},
            {"atk", 31},
            {"def", 31},
            {"spa", 31},
            {"spd", 31},
            {"spe", 31}
        };
        public Dictionary<string, int> Evs { get; set; } = new Dictionary<string, int>()
        {
            {"hp", 0},
            {"atk", 0},
            {"def", 0},
            {"spa", 0},
            {"spd", 0},
            {"spe", 0}
        };
        public List<string> Moves { get; set; } = new List<string>();
        
        public string IconUrl { get; set; }


        public TeamPokemonModel ConvertFrom(string paste)
        {
            var lines = paste.Split("\n");
            foreach (var line in lines)
            {
                if(line.StartsWith("-"))
                    Moves.Add(line.Split("- ")[1]);
                else if (line.StartsWith("IVs:"))
                {
                    var stats = line.Split(": ")[1].Split(" / ");
                    if (stats is null)
                    {
                        var iv = line.Split(":")[1].Split(" ")[1];
                        var num = Convert.ToInt32(line.Split(":")[1].Split(" ")[0]);
                        Ivs[iv.ToLower()] = num;
                    }
                    else
                        foreach (var stat in stats)
                        {
                            var iv = stat.Split(" ")[1];
                            var num = Convert.ToInt32(stat.Split(" ")[0]);
                            Ivs[iv.ToLower()] = num;
                        }
                }
                else if (line.StartsWith("EVs:"))
                {
                    var stats = line.Split(": ")[1].Split(" / ");
                    if (stats is null)
                    {
                        var ev = line.Split(":")[1].Split(" ")[1];
                        var num = Convert.ToInt32(line.Split(":")[1].Split(" ")[0]);
                        Evs[ev.ToLower()] = num;
                    }
                    else
                        foreach (var stat in stats)
                        {
                            var ev = stat.Split(" ")[1];
                            var num = Convert.ToInt32(stat.Split(" ")[0]);
                            Evs[ev.ToLower()] = num;
                        }
                }
                else if (line.Contains("Nature")) Nature = line.Split(" ")[0];
                else if (line.StartsWith("Ability:")) Ability = line.Split(": ")[1];
                else if (line.Contains("@"))
                {
                    var sections = line.Split(" @ ");
                    if (sections[0].Contains("("))
                    {
                        var names = sections[0].Split("(");
                        Name = names[1].Replace(")", "");
                        NickName = names[0];
                        var namesWithGenders = new List<string>()
                        {
                            "nidoqueen",
                            "nidoking",
                            "miltank"
                        };
                        if (namesWithGenders.Contains(NickName.Trim().ToLower()))
                        {
                            Name = NickName.Trim();
                        }
                    }
                    else
                        Name = sections[0];

                    Item = sections[1];
                }
            }

            return this;
        }

        public override string ToString()
        {
            var set = "";
            if (string.IsNullOrWhiteSpace(NickName))
                set += $"{Name}";
            else set += $"{NickName} ({Name})";
            if (!string.IsNullOrWhiteSpace(Item))
                set += $" @ {Item}";
            set += "\n";
            set += $"Ability: {Ability}\n";
            var str = "";

            foreach (var ev in Evs)
            {
                if (!string.IsNullOrWhiteSpace(str) && ev.Value > 0) str += " / ";
                if (ev.Value > 0) str += $"{ev.Value} {ev.Key.ToUpper()}";
            }

            if (!string.IsNullOrWhiteSpace(str)) set += $"EVs: {str}\n";
            str = "";
            set += $"{Nature} Nature\n";
            foreach (var iv in Ivs)
            {
                if (!string.IsNullOrWhiteSpace(str) && iv.Value < 31) str += " / ";
                if (iv.Value < 31) str += $"{iv.Value} {iv.Key.ToUpper()}";
            }

            if (!string.IsNullOrWhiteSpace(str)) set += $"IVs: {str}\n";

            foreach (var move in Moves)
                set += $"- {move}\n";
            return set;
        }
    }
}