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
        public async Task<StatusCodeResult> Put()
        {
            try {
                var authorizedUser = await _authorizor.Authorize(Request.Headers);

                LoggerManager.Instance.LogDebug("Received request.", authorizedUser.Username);
                if (!_auditorRegistry.HasRegisteredWhisperAuditor(authorizedUser.Username))
                    _auditorRegistry.RegisterNewWhisperAuditor(authorizedUser.Username, authorizedUser.Token, new ExcelWhisperRepository(_excelFileManager));
                var auditor = _auditorRegistry.GetRegisteredWhisperAuditor(authorizedUser.Username);

                StartAuditing(authorizedUser.Username, auditor);
                return Ok();
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Unhandled error encountered.", e);
                return StatusCode(500);
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
