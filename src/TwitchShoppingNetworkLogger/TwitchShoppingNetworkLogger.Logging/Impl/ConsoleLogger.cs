using TwitchShoppingNetworkLogger.Logging.Data;
using TwitchShoppingNetworkLogger.Logging.Interfaces;
using Newtonsoft.Json;
using System;

namespace TwitchShoppingNetworkLogger.Logging.Impl
{
    public class ConsoleLogger : ILogger
    {
        public void LogDebug(string message)
        {
            Log("Debug", message, null);
        }

        public void LogDebug(string message, object objToLog)
        {
            Log("Debug", message, objToLog);
        }

        public void LogError(string message)
        {
            Log("Error", message, null);
        }

        public void LogError(string message, object objToLog)
        {
            Log("Error", message, objToLog);
        }

        public void LogFatal(string message)
        {
            Log("Fatal", message, null);
        }

        public void LogFatal(string message, object objToLog)
        {
            Log("Fatal", message, objToLog);
        }

        public void LogInfo(string message)
        {
            Log("Info", message, null);
        }

        public void LogInfo(string message, object objToLog)
        {
            Log("Info", message, objToLog);
        }

        public void LogWarning(string message)
        {
            Log("Warning", message, null);
        }

        public void LogWarning(string message, object objToLog)
        {
            Log("Warning", message, objToLog);
        }

        private void Log(string level, string message, object objToLog)
        {
            var logMessage = new LogMessage() {
                Level = level,
                Message = message,
                ObjectToLog = objToLog
            };
            Console.WriteLine(JsonConvert.SerializeObject(objToLog));
        }
    }
}
