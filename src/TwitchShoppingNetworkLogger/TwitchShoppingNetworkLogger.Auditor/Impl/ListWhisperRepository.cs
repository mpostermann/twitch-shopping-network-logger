using System;
using System.Collections.Generic;
using System.Linq;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class ListUserModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool IsSubbed { get; set; }
    }

    public class ListWhisperModel
    {
        public DateTime TimeReceived { get; set; }
        public string Message { get; set; }
    }

    public class ListWhisperRepository : IWhisperRepository
    {
        private IDictionary<string, List<ListUserModel>> _userListsBySession;
        private IDictionary<string, IDictionary<string, List<ListWhisperModel>>> _messageListsBySessionAndUser;

        public ListWhisperRepository()
        {
            _userListsBySession = new Dictionary<string, List<ListUserModel>>();
            _messageListsBySessionAndUser = new Dictionary<string, IDictionary<string, List<ListWhisperModel>>>();
        }

        public void LogWhisper(IWhisperMessage whisper)
        {
            List<ListUserModel> users = GetUserListBySession(whisper.SessionId);
            if (!users.Any(n => n.UserId == whisper.FromUserId))
            {
                users.Add(new ListUserModel()
                {
                    IsSubbed = false,
                    UserId = whisper.FromUserId,
                    Username = whisper.FromUsername
                });
            }

            List<ListWhisperModel> whispers = GetWhisperListBySessionAndUser(whisper.SessionId, whisper.FromUserId);
            whispers.Add(new ListWhisperModel()
            {
                TimeReceived = whisper.TimeReceived,
                Message = whisper.Message
            });
        }

        public List<ListUserModel> GetUserListBySession(string sessionId)
        {
            if (!_userListsBySession.ContainsKey(sessionId))
                _userListsBySession.Add(sessionId, new List<ListUserModel>());
            return _userListsBySession[sessionId];
        }

        public List<ListWhisperModel> GetWhisperListBySessionAndUser(string sessionId, string userId)
        {
            if (!_messageListsBySessionAndUser.ContainsKey(sessionId))
                _messageListsBySessionAndUser.Add(sessionId, new Dictionary<string, List<ListWhisperModel>>());
            if (!_messageListsBySessionAndUser[sessionId].ContainsKey(userId))
                _messageListsBySessionAndUser[sessionId].Add(userId, new List<ListWhisperModel>());
            return _messageListsBySessionAndUser[sessionId][userId];
        }
    }
}
