using System.Collections.Generic;

namespace TwitchShoppingNetworkLogger.Config.Interfaces
{
    public interface IConfig
    {
        string TwitchClientKey { get; }
        bool AutoRespondEnabled { get; }
        string FirstWhisperResponse { get; }
        ICollection<string> AuthorizedUsers { get; }
        string ExcelDirectory { get; }
    }
}
