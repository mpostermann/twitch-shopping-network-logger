using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Auditor.Models;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class ExcelUserIndex
    {
        public string UserId { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public ExcelUserIndex(string userId, int row)
        {
            UserId = userId;
            Row = row;
            Col = 2;
        }

        public void IncrementColumn()
        {
            Col++;
        }
    }

    public class ExcelWhisperRepository : IWhisperRepository
    {
        private const string WORKSHEET_NAME = "TSN_Logs";

        // todo: need to store excel packages by session (or open a new package for each write; that might be inefficient though)
        private ExcelPackage _excelPackage;
        private object _excelLock = new Object();

        private IDictionary<string, IList<ExcelUserIndex>> _usersBySession;

        public ExcelWhisperRepository()
        {
            _excelPackage = null;
            _usersBySession = new Dictionary<string, IList<ExcelUserIndex>>();
        }

        public ISession CreateSessionForUser(string userId)
        {
            var retVal = new Session(userId);

            // Create a new file
            var file = new FileInfo($"TSN_{userId}_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.xlsx");
            _excelPackage = CreateNewExcel(file); 
            _excelPackage.Save();

            // Initialize users list
            _usersBySession.Add(retVal.Id, new List<ExcelUserIndex>());

            return retVal;
        }

        private static ExcelPackage CreateNewExcel(FileInfo file)
        {
            var retVal = new ExcelPackage(file);
            var sheet = retVal.Workbook.Worksheets.Add(WORKSHEET_NAME);

            // Add headers and format the worksheet
            sheet.Cells[1, 1].Value = "Users";
            sheet.Cells[1, 2].Value = "Messages";
            sheet.Row(1).Style.Font.Bold = true;
            sheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            sheet.Column(1).Style.Font.Bold = true;
            sheet.Column(1).Width = 25;

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
            var index = GetUserIndex(whisper.FromUserId, whisper.SessionId);
            var sheet = _excelPackage.Workbook.Worksheets[WORKSHEET_NAME];

            // Update the excel sheet with the whisper message
            lock (_excelLock)
            {
                sheet.Cells[index.Row, 1].Value = whisper.FromUsername;
                sheet.Cells[index.Row, index.Col].Value = whisper.Message;
                _excelPackage.Save();
            }

            index.IncrementColumn();
        }

        private ExcelUserIndex GetUserIndex(string userId, string sessionId)
        {
            if (!_usersBySession.ContainsKey(sessionId))
                throw new ArgumentException($"A session with id {sessionId} has not been started.");

            var users = _usersBySession[sessionId];
            if (!users.Any(n => n.UserId == userId))
            {
                var row = users.Count + 2;
                users.Add(new ExcelUserIndex(userId, row));
            }

            return users.First(n => n.UserId == userId);
        }

        public bool HasUserWhisperedYet(string userId, string sessionId)
        {
            if (!_usersBySession.ContainsKey(sessionId))
                throw new ArgumentException($"A session with id {sessionId} has not been started.");

            var users = _usersBySession[sessionId];
            return users.Any(n => n.UserId == userId);
        }
    }
}
