

using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguAccountRequestModel : IuguBaseErrors
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("commission_percent ")]
        public int? CommissionPercent { get; set; }

        [JsonProperty("api_token ")]
        public string ApiToken { get; set; }
    }
}
