
namespace TwitchShoppingNetworkLogger.WebApi.Request
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string username, string token)
        {
            Username = username;
            Token = token;
        }

        public string Username { get; private set; }
        public string Token { get; private set; }
    }
}
