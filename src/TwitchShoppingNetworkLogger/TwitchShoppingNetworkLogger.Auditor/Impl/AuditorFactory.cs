using System.Collections.Generic;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class AuditorFactory : IAuditorFactory
    {
        private static readonly IDictionary<string, IWhisperAuditor> _auditors = new Dictionary<string, IWhisperAuditor>();

        private readonly IUserRepository _userRepository;

        public AuditorFactory(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public IWhisperAuditor GetWhisperAuditor(string username, string oAuthToken)
        {
            if (!_auditors.ContainsKey(username))
            {
                AddWhisperAuditor(username, oAuthToken);
            }
            return _auditors[username];
        }

        private void AddWhisperAuditor(string username, string oAuthToken)
        {
            if (_userRepository.IsUserAuthorized(username))
            {
                var user = _userRepository.GetUserByUsername(username);
                _auditors.Add(username, new WhisperAuditor(user, oAuthToken, new ListWhisperRepository()));
            }
        }
    }
}
