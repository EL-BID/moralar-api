
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class DataTriggerModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("feedback")]
        public string Feedback { get; set; }

        [JsonProperty("charge_limit_cents")]
        public string ChargeLimitCents { get; set; }

        [JsonProperty("withdraw_request_id")]
        public string WithdrawRequestId { get; set; }

        [JsonProperty("lr")]
        public string Lr { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("number_of_installments")]
        public int NumberOfInstallments { get; set; }

        [JsonProperty("installment")]
        public int Installment { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }

        [JsonProperty("expires_at")]
        public string ExpiresAt { get; set; }

        [JsonProperty("plan_identifier")]
        public string PlanIdentifier { get; set; }
    }
}
