using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class Subitem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price_cents")]
        public int PriceCents { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }
    }
}
