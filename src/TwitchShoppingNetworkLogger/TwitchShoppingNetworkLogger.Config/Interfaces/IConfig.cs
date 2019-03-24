using System.Collections.Generic;

namespace TwitchShoppingNetworkLogger.Config.Interfaces
{
    public interface IConfig
    {
        string TwitchClientKey { get; }
        ICollection<string> AuthorizedUsers { get; }
    }
}
