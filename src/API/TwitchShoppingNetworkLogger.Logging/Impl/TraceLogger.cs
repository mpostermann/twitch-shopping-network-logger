using Newtonsoft.Json;
using TwitchShoppingNetworkLogger.Logging.Data;
using TwitchShoppingNetworkLogger.Logging.Interfaces;

namespace TwitchShoppingNetworkLogger.Logging.Impl
{
    public class TraceLogger : ILogger
    {
        public void LogDebug(string message)
        {
            System.Diagnostics.Trace.TraceInformation(
                GetLogMessage("Debug", message, null));
        }

        public void LogDebug(string message, object objToLog)
        {
            System.Diagnostics.Trace.TraceInformation(
                GetLogMessage("Debug", message, objToLog));
        }

        public void LogError(string message)
        {
            System.Diagnostics.Trace.TraceError(
                GetLogMessage("Error", message, null));
        }

        public void LogError(string message, object objToLog)
        {
            System.Diagnostics.Trace.TraceError(
                GetLogMessage("Error", message, objToLog));
        }

        public void LogFatal(string message)
        {
            System.Diagnostics.Trace.Fail(
                GetLogMessage("Fatal", message, null));
        }

        public void LogFatal(string message, object objToLog)
        {
            System.Diagnostics.Trace.Fail(
                GetLogMessage("Fatal", message, objToLog));
        }

        public void LogInfo(string message)
        {
            System.Diagnostics.Trace.TraceInformation(
                GetLogMessage("Info", message, null));
        }

        public void LogInfo(string message, object objToLog)
        {
            System.Diagnostics.Trace.TraceInformation(
                GetLogMessage("Info", message, objToLog));
        }

        public void LogWarning(string message)
        {
            System.Diagnostics.Trace.TraceWarning(
                GetLogMessage("Warning", message, null));
        }

        public void LogWarning(string message, object objToLog)
        {
            System.Diagnostics.Trace.TraceWarning(
                GetLogMessage("Warning", message, objToLog));
        }

        private string GetLogMessage(string level, string message, object objToLog)
        {
            var logMessage = new LogMessage() {
                Level = level,
                Message = message,
                ObjectToLog = objToLog
            };
            return JsonConvert.SerializeObject(logMessage);
        }
    }
}
