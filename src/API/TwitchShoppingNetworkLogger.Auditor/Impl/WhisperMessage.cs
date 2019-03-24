using System;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class WhisperMessage : IWhisperMessage
    {
        public string Id { get; set; }
        public string ToUserId { get; set; }
        public string FromUserId { get; set; }
        public string FromUsername { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
        public DateTime TimeReceived { get; set; }

        public WhisperMessage(string id, string toUserId, string fromUserId, string fromUsername, string sessionId, string message)
        {
            Id = id;
            ToUserId = toUserId;
            FromUserId = fromUserId;
            FromUsername = fromUsername;
            SessionId = sessionId;
            Message = message;
            TimeReceived = DateTime.Now;
        }
    }
}
