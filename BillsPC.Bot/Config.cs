using System.IO;
using Newtonsoft.Json;

namespace BillsPC.Bot
{
    /// <summary>
    /// This is unneeded, but will remain here.
    /// This was to handle the configuration for the bot,
    /// but since the dashboard is going to supply them,
    /// we don't need to have this class.
    /// </summary>
    public static class Config
    {
        private static string ConfigFolder = "Resources";
        private static string ConfigFile = "config.json";
        private static string ConfigPath = ConfigFolder + "/" + ConfigFile;
        public static BotConfig Bot;
        static Config()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            if (!File.Exists(ConfigPath))
            {
                Bot = new BotConfig();
                var json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigPath);
                Bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
        [JsonProperty("airtable_api_key")]
        public string AirtableApiKey { get; set; }
        [JsonProperty("airtable_base_id")]
        public string AirtableBaseId { get; set; }
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}