using System;
using System.Collections.Generic;
using System.Linq;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Auditor.Models;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class AggregateWhisperRepository : IWhisperRepository
    {
        private IWhisperRepository[] _repositories;
        private IDictionary<string, ISession[]> _sessionsMap; 

        public AggregateWhisperRepository(ICollection<IWhisperRepository> repositories)
        {
            _repositories = repositories.ToArray();
            _sessionsMap = new Dictionary<string, ISession[]>();
        }

        public ISession CreateSessionForUser(string userId)
        {
            var session = new Session(userId);
            var innerSessions = new ISession[_repositories.Length];

            for (int i = 0; i < _repositories.Length; i++)
            {
                var innerSession = _repositories[i].CreateSessionForUser(userId);
                innerSessions[i] = innerSession;
            }

            _sessionsMap.Add(session.Id, innerSessions);
            return session;
        }

        public void CloseSession(string sessionId)
        {
            IList<Exception> exceptions = new List<Exception>();

            for (int i = 0; i < _repositories.Length; i++)
            {
                try
                {
                    var innerSession = _sessionsMap[sessionId][i];
                    _repositories[i].CloseSession(innerSession.Id);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
                throw new AggregateException("Failed to close one or more Whisper Repository sessions.", exceptions);
        }

        public void LogWhisper(IWhisperMessage whisper)
        {
            IList<Exception> exceptions = new List<Exception>();

            for (int i = 0; i < _repositories.Length; i++)
            {
                try
                {
                    var innerSession = _sessionsMap[whisper.SessionId][i];
                    var innerWhisper = CopyWhisper(whisper, innerSession.Id);
                    
                    _repositories[i].LogWhisper(innerWhisper);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
                throw new AggregateException("Failed to log one or more Whisper Messages.", exceptions);
        }

        private static IWhisperMessage CopyWhisper(IWhisperMessage whisper, string newSessionId)
        {
            var retVal = new WhisperMessage(whisper.Id, whisper.ToUserId, whisper.FromUserId, whisper.FromUsername, newSessionId, whisper.Message);
            retVal.TimeReceived = whisper.TimeReceived;
            return retVal;
        }

        public bool HasUserWhisperedYet(string userId, string sessionId)
        {
            var repository = _repositories[0];
            var innerSession = _sessionsMap[sessionId][0];

            return repository.HasUserWhisperedYet(userId, innerSession.Id);
        }

        public IList<ISession> GetInnerSessions(string sessionId)
        {
            return new List<ISession>(_sessionsMap[sessionId]);
        }
    }
}
