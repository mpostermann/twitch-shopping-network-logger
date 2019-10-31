using System;
using System.Threading.Tasks;
using Logging;
using Microsoft.AspNetCore.Mvc;
using TwitchShoppingNetworkLogger.Auditor.Impl;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config;
using TwitchShoppingNetworkLogger.Excel;
using TwitchShoppingNetworkLogger.WebApi.Auth;

namespace TwitchShoppingNetworkLogger.WebApi.Controllers
{
    [ApiController]
    [Route("api/startlogging")]
    public class StartLoggingController : TSNControllerBase
    {
        private IAuthorizor _authorizor;
        private IUserRepository _userRepository;
        private IAuditorRegistry _auditorRegistry;
        private ExcelFileManager _excelFileManager;

        public StartLoggingController()
        {
            _authorizor = new TwitchAuthorizor(ConfigManager.Instance.AuthorizedUsers);
            _userRepository = new UserRepository(ConfigManager.Instance);
            _auditorRegistry = new AuditorRegistry(_userRepository, ConfigManager.Instance);
            _excelFileManager = new ExcelFileManager(ConfigManager.Instance.ExcelDirectory);
        }

        [HttpPut]
        public async Task<string> Put()
        {
            try {
                var request = await _authorizor.Authorize(Request.Headers);

                LoggerManager.Instance.LogDebug("Received request.", request.Username);
                if (!_auditorRegistry.HasRegisteredWhisperAuditor(request.Username))
                    _auditorRegistry.RegisterNewWhisperAuditor(request.Username, request.Token, new ExcelWhisperRepository(_excelFileManager));
                var auditor = _auditorRegistry.GetRegisteredWhisperAuditor(request.Username);

                StartAuditing(request.Username, auditor);
                return "Success";
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Unhandled error encountered.", e);
                throw;
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
