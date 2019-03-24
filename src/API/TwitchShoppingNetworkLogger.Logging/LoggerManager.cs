using TwitchShoppingNetworkLogger.Logging.Impl;
using TwitchShoppingNetworkLogger.Logging.Interfaces;

namespace Logging
{
    public class LoggerManager
    {
        private static ILogger _loggerInstance;

        public static ILogger Instance
        {
            get
            {
                if (_loggerInstance == null)
                    _loggerInstance = new ConsoleLogger();
                return _loggerInstance;
            }
        }
    }
}
