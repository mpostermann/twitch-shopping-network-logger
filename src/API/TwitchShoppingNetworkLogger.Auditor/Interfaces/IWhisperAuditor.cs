
using System;

namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IWhisperAuditor
    {
        bool IsAuditing();
        Guid CurrentSessionId { get; }
        void StartAuditing();
        void EndAuditing();
    }
}
