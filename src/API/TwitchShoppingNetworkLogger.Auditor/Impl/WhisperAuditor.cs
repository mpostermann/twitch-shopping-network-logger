﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config.Interfaces;
using WhisperMessage = TwitchShoppingNetworkLogger.Auditor.Models.WhisperMessage;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class WhisperAuditor : IWhisperAuditor
    {
        private readonly IWhisperRepository _repository;
        private readonly bool _autoRespondEnabled;
        private readonly string _firstWhisperResponse;

        private ISession _currentSession;
        private TwitchClient _client;

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
            var credentials = new ConnectionCredentials(User.Username, oAuthToken);
            _client = CreateClient(credentials, User.Username);
        }

        private TwitchClient CreateClient(ConnectionCredentials credentials, string username)
        {
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 100,
                ThrottlingPeriod = TimeSpan.FromSeconds(60)
            };
            var customClient = new WebSocketClient(clientOptions);

            var retVal = new TwitchClient(customClient);
            retVal.Initialize(credentials, username);

            retVal.OnConnected += Client_OnConnected;
            retVal.OnJoinedChannel += Client_OnJoinedChannel;
            retVal.OnWhisperReceived += Client_OnWhisperReceived;
            retVal.OnDisconnected += Client_OnDisconnected;

            return retVal;
        }

        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            if (IsAuditing())
            {
                // If we disconnected while auditing, that means we unexpectedly lost connection. Attempt to reconnect.
                LoggerManager.Instance.LogError("Logger unexpectedly disconnected from Twitch. Attempting to reconnect...");

                while (!_client.IsConnected && IsAuditing())
                {
                    Exception connectionException = null;
                    try {
                        LoggerManager.Instance.LogInfo("Attempting reconnect...");
                        ReconnectClient();
                    }
                    catch (Exception ex) {
                        connectionException = ex;
                    }

                    if (!_client.IsConnected && IsAuditing())
                    {
                        LoggerManager.Instance.LogError($"Reconnect failed. Attempting again in 15 seconds...", connectionException);
                        Task.Delay(15000);
                    }
                }
            }
        }

        private void ReconnectClient()
        {
            /* When reconnecting due to a network error, we need to fully recreate a new web socket to reconnect properly.
             * Simply reconnecting the existing TwitchClient doesn't work.
             */
            _client.OnDisconnected -= Client_OnDisconnected;
            _client.Disconnect();

            var credentials = _client.ConnectionCredentials;
            _client = CreateClient(credentials, User.Username);
            _client.Connect();
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

            bool isFirstWhisper = !_repository.HasUserWhisperedYet(whisper.FromUserId, whisper.SessionId);
            LogWhisper(whisper);

            if (isFirstWhisper)
                MessageUserForFirstWhisper(whisper.FromUserId, whisper.FromUsername);
        }

        private void MessageUserForFirstWhisper(string userId, string username)
        {
            try
            {
                if (_autoRespondEnabled)
                {
                    LoggerManager.Instance.LogInfo($"Received first message from '{userId}'; replying with message.");

                    /* If a user is set to only receive whispers from users they've whispered before,
                       then you may need to wait a couple seconds before the setting updates on Twitch's side */
                    Thread.Sleep(2000);
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
            _repository.CloseSession(_currentSession.Id);
            _currentSession = null;
            _client.Disconnect();
        }
    }
}
