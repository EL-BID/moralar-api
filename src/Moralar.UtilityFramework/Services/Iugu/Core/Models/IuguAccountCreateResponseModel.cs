

using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguAccountCreateResponseModel : IuguBaseErrors
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("live_api_token")]
        public string LiveApiToken { get; set; }

        [JsonProperty("test_api_token")]
        public string TestApiToken { get; set; }

        [JsonProperty("user_token")]
        public string UserToken { get; set; }
    }
}
