using System.Collections.Generic;

namespace BillsPC.Bot.Global
{
    class Commands
    {
        public static List<GuildSettings> GuildSettings { get; set; } = new List<GuildSettings>()
        {
            new GuildSettings
            {
                GuildId = 714575393285472356,
                Prefix = "b!",
                Enabled = new Dictionary<string, bool>()
                {
                    {"latency", true},
                    {"help", true}
                },
                VerboseMode = false
            }
        };
    }

    public class GuildSettings
    {

        public ulong GuildId { get; set; }
        public Dictionary<string, bool> Enabled { get; set; } = new Dictionary<string, bool>()
        {
            {"latency", true},
            {"help", true}
        };

        public bool VerboseMode { get; set; } = false;
        public string Prefix { get; set; }

        public void SetPrefix(string prefix) => Prefix = prefix;
        public void DisableCommand(string command) => Enabled[command] = false;
        public void EnableCommand(string command) => Enabled[command] = true;
    }
}