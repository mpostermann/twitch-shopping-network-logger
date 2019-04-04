using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Api;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Auditor.Models;
using TwitchShoppingNetworkLogger.Config.Interfaces;
using TwitchShoppingNetworkLogger.Db.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class UserRepository : IUserRepository
    {
        private string _clientKey;
        private string[] _authorizedUsers;

        public UserRepository(IConfig config, IDbContext dbContext)
        {
            _clientKey = config.TwitchClientKey;

            // TODO: Load authorized users from the database once implemented
            //_authorizedUsers = GetAuthorizedUsers(dbContext);
            _authorizedUsers = GetAuthorizedUsersFromConfig(config);
        }

        private static string[] GetAuthorizedUsers(IDbContext dbContext)
        {
            var users = dbContext.Get<AuthorizedUser>(new object());

            var retVal = new List<string>();
            foreach (var user in users)
                retVal.Add(user.Username);

            return retVal.ToArray();
        }

        private static string[] GetAuthorizedUsersFromConfig(IConfig config)
        {
            return config.AuthorizedUsers.ToArray();
        }

        public IUser GetUserByUsername(string username)
        {
            // TODO: Wrap the Twitch API in an interface so we can write unit tests
            var api = new TwitchAPI();
            api.Settings.ClientId = _clientKey;

            var users = api.Helix.Users.GetUsersAsync(null, new List<string> { username }).Result;
            if (users == null || users.Users.Length == 0)
                throw new ArgumentException($"User with name {username} is not registered on Twitch.");
            
            return new User(users.Users[0].Id, username);
        }

        public bool IsUserAuthorized(string username)
        {
            return _authorizedUsers.Any(n => username.ToLower().Equals(n));
        }
    }
}
