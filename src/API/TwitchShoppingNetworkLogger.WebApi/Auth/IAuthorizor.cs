using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TwitchShoppingNetworkLogger.WebApi.Request;

namespace TwitchShoppingNetworkLogger.WebApi.Auth
{
    public interface IAuthorizor
    {
        Task<AuthorizationRequest> Authorize(IHeaderDictionary headers);
    }
}
