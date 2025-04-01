using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguPlanModel : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("interval_type")]
        public string IntervalType { get; set; }

        [JsonProperty("payable_with")]
        public string PayableWith { get; set; }

        [JsonProperty("prices")]
        public List<IuguPricesModel> Prices { get; set; }

        [JsonIgnore]
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
