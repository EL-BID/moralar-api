using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguTrasferValuesModel
    {
        [JsonProperty("receiver_id")]
        public string Receive { get; set; }

        [JsonProperty("amount_cents")]
        public int AmoutCents { get; set; }
    }
}
