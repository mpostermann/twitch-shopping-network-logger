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

            // Check if the token is valid
            if (response.Status == 401)
                return new AuthorizationRequest(string.Empty, string.Empty, 401);

            // Validate that our user is allowed to use our application
            if (!_authorizedUsernames.Contains(response.Login))
                return new AuthorizationRequest(response.Login, oauth, 403);

            return new AuthorizationRequest(response.Login, oauth, 200);
        }

        private async Task<TwitchAuthValidateResponse> ValidateToken(string oauth)
        {
            /* Send an HTTP request to Twitch's oauth endpoint.
             * There's probably a way to do this through the TwitchLib library but I cannot
             * find an appropriate method through the documentation or browsing the code
             */
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"OAuth {oauth}");
            HttpResponseMessage response = await client.GetAsync(TwitchAuthUrl);

            // Check if the token is valid, or if there was some other error
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return new TwitchAuthValidateResponse() { Status = 401 };
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Authentication failed with result {response.StatusCode}");

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TwitchAuthValidateResponse>(responseContent);
        }
    }
}
