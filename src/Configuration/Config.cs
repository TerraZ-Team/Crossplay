using System.IO;
using TShockAPI;
using TShockAPI.Configuration;

namespace Crossplay.Configuration
{
    static class Config
    {
        private static readonly string _configFilePath = Path.Combine(TShock.SavePath, "Crossplay.json");
        private static ConfigFile<ConfigSettings> _config = new();
        public static ConfigSettings Settings => _config.Settings;

        static Config()
        {
            Reload();
        }

        public static void Save()
        {
            _config.Write(_configFilePath);
        }

        public static void Reload()
        {
            _config.Read(_configFilePath, out bool incomplete);
            if (incomplete)
            {
                Settings.SupportJourneyClients = false;
                Save();
            }   
        }
    }
}
