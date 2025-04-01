using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguChargeRequest : IuguBaseErrors
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("customer_payment_method_id")]
        public string CustomerPaymentMethodId { get; set; }

        [JsonProperty("customer_id ")]
        public string CustomerId { get; set; }

        [JsonProperty("invoice_id ")]
        public string InvoiceId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("months")]
        public int? Months { get; set; }

        [JsonProperty("discount_cents")]
        public int? DiscountCents { get; set; }

        [JsonProperty("items")]
        public IuguInvoiceItem[] InvoiceItems { get; set; }

        [JsonProperty("payer")]
        [IsClass]
        public PayerModel PayerCustomer { get; set; }
    }
}
