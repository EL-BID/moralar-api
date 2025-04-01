using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class SubscriptionResponseMessage : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("suspended")]
        public bool Suspended { get; set; }

        [JsonProperty("plan_identifier")]
        public string PlanIdentifier { get; set; }

        [JsonProperty("price_cents")]
        public int PriceCents { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("features")]
        public Dictionary<string, string> Features { get; set; }

        [JsonProperty("expires_at")]
        public string ExpiresAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }

        [JsonProperty("cycled_at")]
        public object CycledAt { get; set; }

        [JsonProperty("credits_min")]
        public int CreditsMin { get; set; }

        [JsonProperty("credits_cycle")]
        public object CreditsCycle { get; set; }

        [JsonProperty("payable_with")]
        public string PayableWith { get; set; }

        [JsonProperty("ignore_due_email")]
        public object IgnoreDueEmail { get; set; }

        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("plan_name")]
        public string PlanName { get; set; }

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("plan_ref")]
        public string PlanRef { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("in_trial")]
        public object InTrial { get; set; }

        [JsonProperty("credits")]
        public int Credits { get; set; }

        [JsonProperty("credits_based")]
        public bool CreditsBased { get; set; }

        [JsonProperty("recent_invoices")]
        public List<RecentInvoices> RecentInvoices { get; set; }

        [JsonProperty("subitems")]
        public List<Subitem> Subitems { get; set; }

        [JsonProperty("logs")]
        public List<Log> Logs { get; set; }

        [JsonProperty("custom_variables")]
        public List<object> CustomVariables { get; set; }
    }
}
