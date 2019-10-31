using Newtonsoft.Json;

namespace TwitchShoppingNetworkLogger.WebApi.Auth
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TwitchAuthValidateResponse
    {
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
    }
}
