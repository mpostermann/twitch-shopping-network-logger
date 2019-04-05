
namespace TwitchShoppingNetworkLogger.Console
{
    public class Parameters
    {
        public string Username { get; private set; }
        public string Token { get; private set; }

        public Parameters(string[] args)
        {
            Username = args[0];
            Token = args[1];
        }
    }
}
