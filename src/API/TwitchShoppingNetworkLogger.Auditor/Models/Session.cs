using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Models
{
    [Table("Sessions", Schema = "twitchsnl")]
    public class Session : ISession
    {
        [Key] [Required] [MaxLength(36)]
        public string Id { get; set; }

        [Required] [MaxLength(40)]
        public string UserId { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Session(string userId)
        {
            Id = Guid.NewGuid().ToString().ToLower();
            UserId = userId;
            StartTime = DateTime.Now;
            EndTime = null;
        }
    }
}
