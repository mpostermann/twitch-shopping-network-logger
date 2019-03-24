
namespace TwitchShoppingNetworkLogger.Logging.Interfaces
{
    public interface ILogger
    {
        void LogDebug(string message);
        void LogDebug(string message, object objToLog);
        void LogInfo(string message);
        void LogInfo(string message, object objToLog);
        void LogWarning(string message);
        void LogWarning(string message, object objToLog);
        void LogError(string message);
        void LogError(string message, object objToLog);
        void LogFatal(string message);
        void LogFatal(string message, object objToLog);
    }
}
