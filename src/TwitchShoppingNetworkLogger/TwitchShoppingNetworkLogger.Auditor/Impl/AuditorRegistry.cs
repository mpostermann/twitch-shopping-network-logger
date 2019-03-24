using System.Collections.Generic;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class AuditorRegistry : IAuditorRegistry
    {
        private static readonly IDictionary<string, IWhisperAuditor> _auditors = new Dictionary<string, IWhisperAuditor>();

        private readonly IUserRepository _userRepository;

        public AuditorRegistry(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public bool HasRegisteredWhisperAuditor(string username)
        {
            return _auditors.ContainsKey(username);
        }

        public IWhisperAuditor GetRegisteredWhisperAuditor(string username)
        {
            return _auditors[username];
        }

        public IWhisperAuditor RegisterNewWhisperAuditor(string username, string oAuthToken, IWhisperRepository repository)
        {
            DisconnectWhisperAuditor(username);
            AddWhisperAuditor(username, oAuthToken, repository);
            return GetRegisteredWhisperAuditor(username);
        }

        private void DisconnectWhisperAuditor(string username)
        {
            if (HasRegisteredWhisperAuditor(username))
            {
                var auditor = GetRegisteredWhisperAuditor(username);
                if (auditor.IsAuditing())
                    auditor.EndAuditing();
            }
        }

        private void AddWhisperAuditor(string username, string oAuthToken, IWhisperRepository repository)
        {
            if (_userRepository.IsUserAuthorized(username)) {
                var user = _userRepository.GetUserByUsername(username);
                _auditors.Add(username, new WhisperAuditor(user, oAuthToken, repository));
            }
        }
    }
}
