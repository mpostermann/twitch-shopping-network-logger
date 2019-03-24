using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public User(string id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}
