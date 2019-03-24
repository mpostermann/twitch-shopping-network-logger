using System;
using System.Timers;
using Logging;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class WhisperAuditor : IWhisperAuditor
    {
        private string _oAuthToken;

        private TwitchClient _client;
        private IWhisperRepository _repository;

        public readonly IUser User;
        public Guid CurrentSessionId { get; private set; }

        public WhisperAuditor(IUser user, string oAuthToken, IWhisperRepository repository)
        {
            _repository = repository;
            User = user;
            _oAuthToken = oAuthToken;
            CurrentSessionId = Guid.Empty;

            // TODO: Wrap TwitchClient in our own interface so we can unit test it
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 100,
                ThrottlingPeriod = TimeSpan.FromSeconds(60)
            };
            var customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            var credentials = new ConnectionCredentials(User.Username, _oAuthToken);
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
            _repository.LogWhisper(whisper);
        }
        
        public bool IsAuditing()
        {
            return !CurrentSessionId.Equals(Guid.Empty);
        }

        public void StartAuditing()
        {
            _client.Connect();
            CurrentSessionId = Guid.NewGuid();
        }

        public void EndAuditing()
        {
            _client.Disconnect();
            CurrentSessionId = Guid.Empty;
        }
    }
}
