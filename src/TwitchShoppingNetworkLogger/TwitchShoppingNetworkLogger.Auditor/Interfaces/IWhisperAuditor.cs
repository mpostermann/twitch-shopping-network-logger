
namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IWhisperAuditor
    {
        bool IsAuditing();
        void StartAuditing();
        void EndAuditing();
    }
}
