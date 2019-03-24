using System;

namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IWhisperMessage
    {
        string Id { get; set; }
        string ToUserId { get; }
        string FromUserId { get; }
        string FromUsername { get; }
        string SessionId { get; }
        string Message { get; }
        DateTime TimeReceived { get; }
    }
}
