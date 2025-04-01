using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class IuguItemResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price_cents")]
        public int? PriceCents { get; set; }

        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }
    }
}