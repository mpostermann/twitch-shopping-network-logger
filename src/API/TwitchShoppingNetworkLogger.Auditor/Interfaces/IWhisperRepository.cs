
namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IWhisperRepository
    {
        ISession CreateSessionForUser(string userId);
        void CloseSession(string sessionId);
        void LogWhisper(IWhisperMessage whisper);
    }
}
