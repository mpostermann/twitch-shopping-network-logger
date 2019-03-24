using TwitchShoppingNetworkLogger.Config.Impl;
using TwitchShoppingNetworkLogger.Config.Interfaces;

namespace TwitchShoppingNetworkLogger.Config
{
    public class ConfigManager
    {
        private static IConfig _instance;

        public static IConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new JsonConfig("Config.json");
                return _instance;
            }
        }
    }
}
