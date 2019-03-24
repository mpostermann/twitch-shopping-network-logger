using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Models
{
    [Table("Users", Schema = "twitchsnl")]
    public class User : IUser
    {
        [Key] [Required] [MaxLength(40)]
        public string Id { get; set; }

        [Required] [MaxLength(100)]
        public string Username { get; set; }

        public User(string id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}
