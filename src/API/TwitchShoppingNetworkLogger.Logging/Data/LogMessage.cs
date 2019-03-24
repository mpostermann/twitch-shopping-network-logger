
namespace TwitchShoppingNetworkLogger.Logging.Data
{
    public class LogMessage
    {
        public string Level { get; set; }
        public string Message { get; set; }
        public object ObjectToLog { get; set; }
    }
}
