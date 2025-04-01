using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class RecentInvoices
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("due_date")]
        public string DueDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("secure_url")]
        public string SecureUrl { get; set; }
    }
}
