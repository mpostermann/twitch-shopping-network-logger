using System.Collections.Generic;
using TwitchShoppingNetworkLogger.Config.Data;

namespace TwitchShoppingNetworkLogger.Config.Interfaces
{
    public interface IConfig
    {
        string TwitchClientKey { get; }
        bool AutoRespondEnabled { get; }
        string FirstWhisperResponse { get; }
        ICollection<string> AuthorizedUsers { get; }
        DatabaseConnection DatabaseConnection { get; }
        string ExcelDirectory { get; }
    }
}
