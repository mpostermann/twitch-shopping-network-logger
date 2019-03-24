
namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IAuditorFactory
    {
        IWhisperAuditor GetWhisperAuditor(string username, string oAuthToken);
    }
}
