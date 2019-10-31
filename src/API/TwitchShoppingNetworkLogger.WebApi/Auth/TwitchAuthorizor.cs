using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitchShoppingNetworkLogger.WebApi.Auth
{
    public class TwitchAuthorizor : IAuthorizor
    {
        private const string TwitchAuthUrl = "https://id.twitch.tv/oauth2/validate";

        private ICollection<string> _authorizedUsernames;

        public TwitchAuthorizor(ICollection<string> authorizedUsernames)
        {
            _authorizedUsernames = authorizedUsernames;
        }

        public async Task Authorize(string username, string oauth)
        {
            var response = await ValidateToken(oauth);

            // Validate that our user is who they claim they are
            if (!username.Trim().ToLower().Equals(response.Login.Trim().ToLower()))
                throw new Exception("Authentication failed");

            // Validate that our user is allowed to use our application
            if (!_authorizedUsernames.Contains(username.Trim().ToLower()))
                throw new Exception("User unauthorized");
        }

        private async Task<TwitchAuthValidateResponse> ValidateToken(string oauth)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"OAuth {oauth}");
            HttpResponseMessage response = await client.GetAsync(TwitchAuthUrl);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Authentication failed");

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TwitchAuthValidateResponse>(responseContent);
        }
    }
}
