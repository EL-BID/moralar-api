using Newtonsoft.Json;


namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguCustomerPaymentMethod : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("set_as_default")]
        public string SetAsDefault { get; set; }

        [JsonProperty("item_type")]
        public string ItemType { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("errors")]
        public string Erro { get; set; }
    }
}
