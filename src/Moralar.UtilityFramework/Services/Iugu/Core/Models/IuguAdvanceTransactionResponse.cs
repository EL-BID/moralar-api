using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguAdvanceTransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("scheduled_date")]
        public string ScheduledDate { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("taxes")]
        public string Taxes { get; set; }

        [JsonProperty("client_share")]
        public string ClientShare { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("number_of_installments")]
        public int? NumberOfInstallments { get; set; }

        [JsonProperty("installment")]
        public int? Installment { get; set; }

        [JsonProperty("advanced_value")]
        public string AdvancedValue { get; set; }

        [JsonProperty("advance_fee")]
        public string AdvanceFee { get; set; }

        [JsonProperty("received_value")]
        public string ReceivedValue { get; set; }
    }
}
