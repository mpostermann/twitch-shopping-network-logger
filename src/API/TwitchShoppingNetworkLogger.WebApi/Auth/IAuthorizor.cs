using System.Threading.Tasks;

namespace TwitchShoppingNetworkLogger.WebApi.Auth
{
    public interface IAuthorizor
    {
        Task Authorize(string username, string oauth);
    }
}
