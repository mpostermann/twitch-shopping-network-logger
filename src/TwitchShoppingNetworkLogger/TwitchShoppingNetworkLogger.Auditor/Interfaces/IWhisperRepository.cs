
namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IWhisperRepository
    {
        void LogWhisper(IWhisperMessage whisper);
    }
}
