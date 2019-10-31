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
                    _loggerInstance = ConstructLogger();
                return _loggerInstance;
            }
        }

        private static ILogger ConstructLogger()
        {
            Directory.CreateDirectory("D:\\home\\logs");
            ILogger textLogger = new TextLogger($"D:\\home\\logs\\TSN_Log_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.log");

            ILogger traceLogger = new TraceLogger();
 
            return new AggregateLogger(new [] { textLogger, traceLogger});
        }
    }
}
