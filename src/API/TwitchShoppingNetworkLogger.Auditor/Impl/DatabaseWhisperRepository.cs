using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Microsoft.EntityFrameworkCore.Internal;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Auditor.Models;
using TwitchShoppingNetworkLogger.Db.Interfaces;

namespace TwitchShoppingNetworkLogger.Auditor.Impl
{
    public class DatabaseWhisperRepository : IWhisperRepository
    {
        private IDbContext _dbContext;
        private IList<string> _userIdCache;
        
        public DatabaseWhisperRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _userIdCache = new List<string>();
        }

        public ISession CreateSessionForUser(string userId)
        {
            var model = new Session(userId);

            // TODO: Run this asynchronously after testing is complete
            var insertTask = InsertIntoDatabaseIfNotAlreadyExistsAsync<Session>(model, model.Id);
            insertTask.Wait();

            return model;
        }

        public void CloseSession(string sessionId)
        {
            var session = Enumerable.ToList(_dbContext.Get<Session>(new {Id = sessionId}));
            if (session.Any())
            {
                session[0].EndTime = DateTime.Now;
                var updateTask = _dbContext.UpdateAsync(session, new {Id = sessionId});
                updateTask.Wait();
            }
        }

        public void LogWhisper(IWhisperMessage whisper)
        {
            var whisperModel = new WhisperMessage(whisper.Id, whisper.ToUserId, whisper.FromUserId, whisper.FromUsername, whisper.SessionId, whisper.Message);
            whisperModel.TimeReceived = whisper.TimeReceived;

            var userModel = new User(whisper.Id, whisper.FromUsername);

            try {
                // TODO: Run this asynchronously after testing is complete
                if (!_userIdCache.Contains(userModel.Id))
                {
                    var insertTask = InsertIntoDatabaseIfNotAlreadyExistsAsync<User>(userModel, userModel.Id);
                    insertTask.Wait();
                    _userIdCache.Add(userModel.Id);
                }
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Could not insert user.", e);
            }

            try {
                // TODO: Run this asynchronously after testing is complete
                var insertTask = InsertIntoDatabaseIfNotAlreadyExistsAsync<WhisperMessage>(whisperModel, whisperModel.Id);
                insertTask.Wait();
            }
            catch (Exception e) {
                LoggerManager.Instance.LogError("Could not insert whisper message.", e);
            }
        }

        public bool HasUserWhisperedYet(string userId, string sessionId)
        {
            // TODO: Complete implementation of this
            throw new NotImplementedException();
        }

        private async Task InsertIntoDatabaseIfNotAlreadyExistsAsync<T>(T model, string id)
        {
            try
            {
                if (!AlreadyExists<T>(id))
                    await _dbContext.InsertAsync(model);
            }
            catch (Exception e)
            {
                LoggerManager.Instance.LogDebug($"Model {typeof(T).Name} with Id {id} failed to insert... will attempt insert again.", e);

                try
                {
                    if (!AlreadyExists<T>(id))
                        await _dbContext.InsertAsync(model);
                }
                catch (Exception e2)
                {
                    LoggerManager.Instance.LogError($"Model {typeof(T).Name} with Id {id} failed to insert twice.", e2);
                    throw new DatabaseWhisperRepositoryInsertException(model, $"Could not insert {typeof(T).Name} into database.", e2);
                }
            }
        }

        private bool AlreadyExists<T>(string id)
        {
            var models = _dbContext.Get<T>(new {Id = id});

            if (EnumerableExtensions.Any(models))
            {
                LoggerManager.Instance.LogDebug($"Model {typeof(T).Name} with Id {id} already exists. Skipping insert");
                return true;
            }
            return false;
        }
    }

    public class DatabaseWhisperRepositoryInsertException : Exception
    {
        public object Model { get; private set; }

        public DatabaseWhisperRepositoryInsertException(object model, string message, Exception innerException) :
            base(message, innerException)
        {
            Model = model;
        }
    }
}
