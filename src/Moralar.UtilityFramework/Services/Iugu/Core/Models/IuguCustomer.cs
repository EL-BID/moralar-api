using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguCustomer : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("cc_emails")]
        public object CcEmails { get; set; }

        [JsonProperty("cpf_cnpj")]
        public string CpfCnpj { get; set; }

        [JsonProperty("zip_code")]
        public object ZipCode { get; set; }

        [JsonProperty("number")]
        public object Number { get; set; }

        [JsonProperty("complement")]
        public object Complement { get; set; }

        [JsonProperty("default_payment_method_id")]
        public object DefaultPaymentMethodId { get; set; }

        [JsonProperty("proxy_payments_from_customer_id")]
        public object ProxyPaymentsFromCustomerId { get; set; }

        [JsonProperty("city")]
        public object City { get; set; }

        [JsonProperty("state")]
        public object State { get; set; }

        [JsonProperty("district")]
        public object District { get; set; }

        [JsonProperty("street")]
        public object Street { get; set; }

        [JsonProperty("custom_variables")]
        public List<object> CustomVariables { get; set; }
    }
}
