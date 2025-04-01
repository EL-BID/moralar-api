using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguAdvanceSimulationResponse : IuguBaseErrors
    {
        [JsonProperty("transactions")]
        public List<IuguAdvanceTransactionResponse> Transactions { get; set; }

        [JsonProperty("total")]
        public IuguTotalAdvanceSimulationResponse Total { get; set; }

        public IuguAdvanceSimulationResponse()
        {
            Transactions = new List<IuguAdvanceTransactionResponse>();
        }
    }
}
