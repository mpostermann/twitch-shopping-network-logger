using System.Collections.Generic;
using TwitchShoppingNetworkLogger.Logging.Interfaces;

namespace TwitchShoppingNetworkLogger.Logging.Impl
{
    public class AggregateLogger : ILogger
    {
        private readonly ICollection<ILogger> _loggers;

        public AggregateLogger(ICollection<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public void LogDebug(string message)
        {
            foreach (var logger in _loggers)
                logger.LogDebug(message);
        }

        public void LogDebug(string message, object objToLog)
        {
            foreach (var logger in _loggers)
                logger.LogDebug(message, objToLog);
        }

        public void LogError(string message)
        {
            foreach (var logger in _loggers)
                logger.LogError(message);
        }

        public void LogError(string message, object objToLog)
        {
            foreach (var logger in _loggers)
                logger.LogError(message, objToLog);
        }

        public void LogFatal(string message)
        {
            foreach (var logger in _loggers)
                logger.LogFatal(message);
        }

        public void LogFatal(string message, object objToLog)
        {
            foreach (var logger in _loggers)
                logger.LogFatal(message, objToLog);
        }

        public void LogInfo(string message)
        {
            foreach (var logger in _loggers)
                logger.LogInfo(message);
        }

        public void LogInfo(string message, object objToLog)
        {
            foreach (var logger in _loggers)
                logger.LogInfo(message, objToLog);
        }

        public void LogWarning(string message)
        {
            foreach (var logger in _loggers)
                logger.LogWarning(message);
        }

        public void LogWarning(string message, object objToLog)
        {
            foreach (var logger in _loggers)
                logger.LogWarning(message, objToLog);
        }
    }
}
