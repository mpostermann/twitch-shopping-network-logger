using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Auditor.Models;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class ExcelWhisperRepository : IWhisperRepository
    {
        private const string WORKSHEET_NAME = "TSN_Logs";

        private ExcelPackage _excelPackage;

        private IDictionary<string, IList<string>> _usersBySession;

        public ExcelWhisperRepository()
        {
            _excelPackage = null;
            _usersBySession = new Dictionary<string, IList<string>>();
        }

        public ISession CreateSessionForUser(string userId)
        {
            var retVal = new Session(userId);

            // Create a new file
            var file = new FileInfo($"TSN_{userId}_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.xlsx");
            // todo: need to store excel packages by session (or open a new package for each write; that might be inefficient though)
            _excelPackage = new ExcelPackage(file);
            _excelPackage.Workbook.Worksheets.Add(WORKSHEET_NAME);
            _excelPackage.Save();

            // Initialize users list
            _usersBySession.Add(retVal.Id, new List<string>());

            return retVal;
        }

        public void CloseSession(string sessionId)
        {
            if (_excelPackage != null)
            {
                _excelPackage.Save();
                _excelPackage.Dispose();
                _excelPackage = null;
            }
        }

        public void LogWhisper(IWhisperMessage whisper)
        {
            int rowIndex = GetUserIndex(whisper.FromUserId, whisper.SessionId) + 1;
            var sheet = _excelPackage.Workbook.Worksheets[WORKSHEET_NAME];
            sheet.Cells[rowIndex, 1].Value = whisper.FromUsername;
            _excelPackage.Save();
        }

        private int GetUserIndex(string userId, string sessionId)
        {
            if (!_usersBySession.ContainsKey(sessionId))
                throw new ArgumentException($"A session with id {sessionId} has not been started.");

            var users = _usersBySession[sessionId];
            if (!users.Contains(userId))
                users.Add(userId);
            return users.IndexOf(userId);
        }

        public bool HasUserWhisperedYet(string userId, string sessionId)
        {
            if (!_usersBySession.ContainsKey(sessionId))
                throw new ArgumentException($"A session with id {sessionId} has not been started.");

            var users = _usersBySession[sessionId];
            return users.Contains(userId);
        }
    }
}
