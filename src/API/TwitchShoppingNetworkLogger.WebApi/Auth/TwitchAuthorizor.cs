using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchShoppingNetworkLogger.WebApi.Request;

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

        public async Task<AuthorizationRequest> Authorize(IHeaderDictionary headers)
        {
            string oauth = headers["Token"];
            var response = await ValidateToken(oauth);

            // Validate that our user is allowed to use our application
            if (!_authorizedUsernames.Contains(response.Login))
                throw new Exception("User unauthorized");

            return new AuthorizationRequest(response.Login, oauth);
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
