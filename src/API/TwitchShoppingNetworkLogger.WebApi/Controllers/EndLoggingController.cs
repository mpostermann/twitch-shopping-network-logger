using System;
using Logging;
using Microsoft.AspNetCore.Mvc;
using TwitchShoppingNetworkLogger.Auditor.Impl;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config;
using TwitchShoppingNetworkLogger.WebApi.Request;

namespace TwitchShoppingNetworkLogger.WebApi.Controllers
{
    [Route("api/endlogging")]
    [ApiController]
    public class EndLoggingController : TSNControllerBase
    {
        private IUserRepository _userRepository;
        private IAuditorRegistry _auditorRegistry;

        public EndLoggingController()
        {
            _userRepository = new UserRepository(ConfigManager.Instance, null);
            _auditorRegistry = new AuditorRegistry(_userRepository);
        }

        [HttpPut]
        public string Put(EndLoggingRequest request)
        {
            try {
                LoggerManager.Instance.LogDebug("Received request.", request.Username);

                var auditor = _auditorRegistry.GetRegisteredWhisperAuditor(request.Username);

                EndAuditing(request.Username, auditor);
                return "Success";
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Unhandled error encountered.", e);
                throw new Exception("Unexpected error.");
            }
        }

        private void EndAuditing(string username, IWhisperAuditor auditor)
        {
            if (!auditor.IsAuditing()) {
                LoggerManager.Instance.LogInfo($"{username} is not currently auditing for whispers.");
            }
            else {
                LoggerManager.Instance.LogInfo($"Attempt to end auditing whispers for {username}...");
                auditor.EndAuditing();
                LoggerManager.Instance.LogInfo($"Successfully stopped auditing whispers for {username}.");
            }
        }
    }
}
