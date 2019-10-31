
namespace TwitchShoppingNetworkLogger.WebApi.Request
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string username, string token, int status)
        {
            Username = username;
            Token = token;
            Status = status;
        }

        public string Username { get; private set; }
        public string Token { get; private set; }
        public int Status { get; private set; } 
    }
}
