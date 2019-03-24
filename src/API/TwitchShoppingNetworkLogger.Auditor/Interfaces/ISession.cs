using System;

namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface ISession
    {
        string Id { get; }
        string UserId { get; }
        DateTime? StartTime { get; }
        DateTime? EndTime { get; }
    }
}
