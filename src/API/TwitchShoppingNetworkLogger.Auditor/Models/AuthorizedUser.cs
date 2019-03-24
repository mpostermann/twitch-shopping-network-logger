using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitchShoppingNetworkLogger.Auditor.Models
{
    [Table("AuthorizedUsers", Schema = "twitchsnl")]
    public class AuthorizedUser
    {
        [Key] [Required] [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public bool IsAuditing { get; set; }
    }
}
