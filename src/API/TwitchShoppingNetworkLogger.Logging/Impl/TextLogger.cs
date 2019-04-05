using Newtonsoft.Json;
using NLog;
using TwitchShoppingNetworkLogger.Logging.Data;
using ILogger = TwitchShoppingNetworkLogger.Logging.Interfaces.ILogger;

namespace TwitchShoppingNetworkLogger.Logging.Impl
{
    public class TextLogger : ILogger
    {
        private NLog.ILogger _logger;

        public TextLogger(string filename)
        {
            ConfigureNLog(filename);
            _logger = NLog.LogManager.GetLogger("logfile");
        }

        private void ConfigureNLog(string filename)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = filename };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
        }

        public void LogDebug(string message)
        {
            Log(LogLevel.Debug, message, null);
        }

        public void LogDebug(string message, object objToLog)
        {
            Log(LogLevel.Debug, message, objToLog);
        }

        public void LogInfo(string message)
        {
            Log(LogLevel.Info, message, null);
        }

        public void LogInfo(string message, object objToLog)
        {
            Log(LogLevel.Info, message, objToLog);
        }

        public void LogWarning(string message)
        {
            Log(LogLevel.Warn, message, null);
        }

        public void LogWarning(string message, object objToLog)
        {
            Log(LogLevel.Warn, message, objToLog);
        }

        public void LogError(string message)
        {
            Log(LogLevel.Error, message, null);
        }

        public void LogError(string message, object objToLog)
        {
            Log(LogLevel.Error, message, objToLog);
        }

        public void LogFatal(string message)
        {
            Log(LogLevel.Fatal, message, null);
        }

        public void LogFatal(string message, object objToLog)
        {
            Log(LogLevel.Fatal, message, objToLog);
        }

        private void Log(LogLevel level, string message, object objToLog)
        {
            var logMessage = new LogMessage()
            {
                Level = level.ToString(),
                Message = message,
                ObjectToLog = objToLog
            };
            _logger.Log(level, JsonConvert.SerializeObject(logMessage));
        }
    }
}
