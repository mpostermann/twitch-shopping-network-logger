using System;
using System.IO;
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
                    _loggerInstance = ConstructTextLogger();
                return _loggerInstance;
            }
        }

        private static ILogger ConstructTextLogger()
        {
            Directory.CreateDirectory("logs");
            return new TextLogger($"logs\\TSN_Log_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.log");
        }
    }
}
