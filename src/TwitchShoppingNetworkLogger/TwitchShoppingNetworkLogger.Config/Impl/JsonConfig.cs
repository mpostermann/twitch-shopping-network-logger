using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitchShoppingNetworkLogger.Config.Interfaces;

namespace TwitchShoppingNetworkLogger.Config.Impl
{
    public class JsonConfig : IConfig
    {
        private JObject _config;

        public JsonConfig(string filepath)
        {
            LoggerManager.Instance.LogDebug($"Loading config from {filepath}");
            _config = JObject.Parse(File.ReadAllText(filepath));
            LoggerManager.Instance.LogDebug("");
        }

        public string TwitchClientKey => _config["TwitchClientKey"].Value<string>();
        public ICollection<string> AuthorizedUsers => _config["AuthorizedUsers"].ToObject<string[]>();
    }
}
