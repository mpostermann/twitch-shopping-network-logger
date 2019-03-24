using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Models
{
    [Table("WhisperMessages", Schema = "twitchsnl")]
    public class WhisperMessage : IWhisperMessage
    {
        [Key] [Required] [MaxLength(40)]
        public string Id { get; set; }

        [Required] [MaxLength(40)]
        public string ToUserId { get; set; }

        [Required] [MaxLength(40)]
        public string FromUserId { get; set; }

        [NotMapped]
        public string FromUsername { get; set; }

        [Required] [MaxLength(36)]
        public string SessionId { get; set; }

        [Required] [MaxLength(500)]
        public string Message { get; set; }

        [Required]
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
