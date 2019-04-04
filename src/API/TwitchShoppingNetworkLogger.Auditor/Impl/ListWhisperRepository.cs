using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TwitchShoppingNetworkLogger.Auditor.Binding;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Auditor.Models;

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
        private ISynchronizeInvoke _userInvoke;
        private ISynchronizeInvoke _whisperInvoke;

        private IDictionary<string, BindingListInvoked<ListUserModel>> _userListsBySession;
        private IDictionary<string, IDictionary<string, BindingMessageListInvoked>> _messageListsBySessionAndUser;

        public ListWhisperRepository() :
            this(null, null)
        {
        }

        public ListWhisperRepository(ISynchronizeInvoke userListInvoke, ISynchronizeInvoke whisperListInvoke)
        {
            _userInvoke = userListInvoke;
            _whisperInvoke = whisperListInvoke;
            _userListsBySession = new Dictionary<string, BindingListInvoked<ListUserModel>>();
            _messageListsBySessionAndUser = new Dictionary<string, IDictionary<string, BindingMessageListInvoked>>();
        }

        public ISession CreateSessionForUser(string userId)
        {
            return new Session(userId);
        }

        public void CloseSession(string sessionId)
        {
            // do nothing
        }

        public void LogWhisper(IWhisperMessage whisper)
        {
            var users = GetUserListBySession(whisper.SessionId);
            if (!users.Any(n => n.UserId == whisper.FromUserId))
            {
                users.Add(new ListUserModel()
                {
                    IsSubbed = false,
                    UserId = whisper.FromUserId,
                    Username = whisper.FromUsername
                });
            }

            var whispers = GetWhisperListBySessionAndUser(whisper.SessionId, whisper.FromUserId);
            whispers.Add(new ListWhisperModel()
            {
                TimeReceived = whisper.TimeReceived,
                Message = whisper.Message
            });
        }

        public bool HasUserWhisperedYet(string userId, string sessionId)
        {
            var users = GetUserListBySession(sessionId);
            return users.Any(n => n.UserId == userId);
        }

        public BindingListInvoked<ListUserModel> GetUserListBySession(string sessionId)
        {
            if (!_userListsBySession.ContainsKey(sessionId))
                _userListsBySession.Add(sessionId, new BindingListInvoked<ListUserModel>(_userInvoke));
            return _userListsBySession[sessionId];
        }

        public BindingMessageListInvoked GetWhisperListBySessionAndUser(string sessionId, string userId)
        {
            if (!_messageListsBySessionAndUser.ContainsKey(sessionId))
                _messageListsBySessionAndUser.Add(sessionId, new Dictionary<string, BindingMessageListInvoked>());
            if (!_messageListsBySessionAndUser[sessionId].ContainsKey(userId))
                _messageListsBySessionAndUser[sessionId].Add(userId, new BindingMessageListInvoked(_whisperInvoke));
            return _messageListsBySessionAndUser[sessionId][userId];
        }
    }
}
