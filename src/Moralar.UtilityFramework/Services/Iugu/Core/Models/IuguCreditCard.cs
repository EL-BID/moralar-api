using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguCreditCard : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("item_type")]
        public string ItemType { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
