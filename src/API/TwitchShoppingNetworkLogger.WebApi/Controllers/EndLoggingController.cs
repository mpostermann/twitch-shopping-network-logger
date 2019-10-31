using System;
using System.IO;
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
    [Route("api/endlogging")]
    public class EndLoggingController : TSNControllerBase
    {
        private IAuthorizor _authorizor;
        private IUserRepository _userRepository;
        private IAuditorRegistry _auditorRegistry;
        private ExcelFileManager _excelFileManager;

        public EndLoggingController()
        {
            _authorizor = new TwitchAuthorizor(ConfigManager.Instance.AuthorizedUsers);
            _userRepository = new UserRepository(ConfigManager.Instance);
            _auditorRegistry = new AuditorRegistry(_userRepository, ConfigManager.Instance);
            _excelFileManager = new ExcelFileManager(ConfigManager.Instance.ExcelDirectory);
        }

        [HttpPut]
        public async Task<Stream> Put()
        {
            try {
                var request = await _authorizor.Authorize(Request.Headers);

                LoggerManager.Instance.LogDebug("Received request.", request.Username);
                var auditor = _auditorRegistry.GetRegisteredWhisperAuditor(request.Username);

                var sessionId = auditor.CurrentSessionId;
                EndAuditing(request.Username, auditor);
                return GetExcelStream(sessionId.ToString());
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Unhandled error encountered.", e);
                throw;
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

        private Stream GetExcelStream(string sessionId)
        {
            // TODO: Move this to a method of the Excel auditor instead of using hard-coded paths
            var path = _excelFileManager.GetFileInfo(sessionId).FullName;
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
