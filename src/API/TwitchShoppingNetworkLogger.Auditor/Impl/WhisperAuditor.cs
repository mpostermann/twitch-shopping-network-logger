using System;
using Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config.Interfaces;
using WhisperMessage = TwitchShoppingNetworkLogger.Auditor.Models.WhisperMessage;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class WhisperAuditor : IWhisperAuditor
    {
        private readonly TwitchClient _client;
        private readonly IWhisperRepository _repository;
        private readonly bool _autoRespondEnabled;
        private readonly string _firstWhisperResponse;

        private ISession _currentSession;

        public readonly IUser User;

        public Guid CurrentSessionId
        {
            get
            {
                if (_currentSession != null)
                    return new Guid(_currentSession.Id);
                return Guid.Empty;
            }
        }

        public WhisperAuditor(IUser user, string oAuthToken, IWhisperRepository repository, IConfig config)
        {
            _repository = repository;
            _autoRespondEnabled = config.AutoRespondEnabled;
            _firstWhisperResponse = config.FirstWhisperResponse;
            User = user;
            _currentSession = null;

            // TODO: Wrap TwitchClient in our own interface so we can unit test this
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 100,
                ThrottlingPeriod = TimeSpan.FromSeconds(60)
            };
            var customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            var credentials = new ConnectionCredentials(User.Username, oAuthToken);
            _client.Initialize(credentials, User.Username);

            _client.OnConnected += Client_OnConnected;
            _client.OnJoinedChannel += Client_OnJoinedChannel;
            _client.OnWhisperReceived += Client_OnWhisperReceived;
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            LoggerManager.Instance.LogInfo($"Connected to channel {User.Username} {User.Id}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            LoggerManager.Instance.LogInfo($"Joined channel {e.Channel}");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            LoggerManager.Instance.LogInfo($"{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
            var whisper = new WhisperMessage(
                e.WhisperMessage.MessageId,
                User.Id,
                e.WhisperMessage.UserId,
                e.WhisperMessage.Username,
                CurrentSessionId.ToString().ToLower(),
                e.WhisperMessage.Message);

            MessageUserIfFirstWhisper(whisper.FromUserId, whisper.FromUsername, whisper.SessionId);
            LogWhisper(whisper);
        }

        private void MessageUserIfFirstWhisper(string userId, string username, string sessionId)
        {
            try
            {
                bool isFirstWhisper = !_repository.HasUserWhisperedYet(userId, sessionId);
                if (_autoRespondEnabled && isFirstWhisper)
                {
                    LoggerManager.Instance.LogInfo($"Received first message from '{userId}'; replying with message.");
                    _client.SendWhisper(username, _firstWhisperResponse);
                }
            }
            catch (Exception e)
            {
                LoggerManager.Instance.LogError($"Unhandled exception while trying to message user '{userId}'.", e);
            }
        }

        private void LogWhisper(WhisperMessage whisper)
        {
            try
            {
                _repository.LogWhisper(whisper);
            }
            catch (Exception e)
            {
                LoggerManager.Instance.LogError("Unhandled exception while trying to log message.", e);
                LoggerManager.Instance.LogError(e.Message, whisper);
            }
        }
        
        public bool IsAuditing()
        {
            return !CurrentSessionId.Equals(Guid.Empty);
        }

        public void StartAuditing()
        {
            _client.Connect();
            _currentSession = _repository.CreateSessionForUser(User.Id);
        }

        public void EndAuditing()
        {
            _client.Disconnect();
            _repository.CloseSession(_currentSession.Id);
            _currentSession = null;
        }
    }
}
