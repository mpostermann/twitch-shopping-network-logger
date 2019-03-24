using System;
using Logging;
using Microsoft.AspNetCore.Mvc;
using TwitchShoppingNetworkLogger.Auditor.Impl;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config;
using TwitchShoppingNetworkLogger.WebApi.Request;

namespace TwitchShoppingNetworkLogger.WebApi.Controllers
{
    [Route("api/startlogging")]
    [ApiController]
    public class StartLoggingController : TSNControllerBase
    {
        private IUserRepository _userRepository;
        private IAuditorRegistry _auditorRegistry;

        public StartLoggingController()
        {
            _userRepository = new UserRepository(ConfigManager.Instance);
            _auditorRegistry = new AuditorRegistry(_userRepository);
        }

        [HttpPut]
        public string Put(StartLoggingRequest request)
        {
            try {
                LoggerManager.Instance.LogDebug("Received request.", request.Username);

                if (!_auditorRegistry.HasRegisteredWhisperAuditor(request.Username))
                    _auditorRegistry.RegisterNewWhisperAuditor(request.Username, request.Token, new ListWhisperRepository());
                var auditor = _auditorRegistry.GetRegisteredWhisperAuditor(request.Username);

                StartAuditing(request.Username, auditor);
                return "Success";
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Unhandled error encountered.", e);
                throw new Exception("Unexpected error.");
            }
        }

        private void StartAuditing(string username, IWhisperAuditor auditor)
        {
            if (auditor.IsAuditing()) {
                LoggerManager.Instance.LogInfo($"{username} is already auditing for whispers.");
            }
            else {
                LoggerManager.Instance.LogInfo($"Attempt to begin auditing whispers for {username}...");
                auditor.StartAuditing();
                LoggerManager.Instance.LogInfo($"Now auditing whispers for {username}.");
            }
        }
    }
}
