
namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IAuditorRegistry
    {
        bool HasRegisteredWhisperAuditor(string username);
        IWhisperAuditor GetRegisteredWhisperAuditor(string username);
        IWhisperAuditor RegisterNewWhisperAuditor(string username, string oAuthToken, IWhisperRepository repository);
    }
}
