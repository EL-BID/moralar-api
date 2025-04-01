using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguTotalAdvanceSimulationResponse
    {
        [JsonProperty("advanced_value")]
        public string AdvancedValue { get; set; }

        [JsonProperty("advance_fee")]
        public string AdvanceFee { get; set; }

        [JsonProperty("received_value")]
        public string ReceivedValue { get; set; }
    }
}
