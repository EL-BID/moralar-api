using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class IuguFinancialReturnDatesResponse
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("installment")]
        public int? Installment { get; set; }

        [JsonProperty("return_date")]
        public string ReturnDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("taxes")]
        public string Taxes { get; set; }

        [JsonProperty("executed_date")]
        public object ExecutedDate { get; set; }

        [JsonProperty("advanced")]
        public bool? Advanced { get; set; }
    }
}