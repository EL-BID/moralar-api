
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguPaymentMethodToken : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("test")]
        public string Test { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("data")]
        [IsClass]
        public IuguDataPaymentMethodToken Data { get; set; }
    }
}
