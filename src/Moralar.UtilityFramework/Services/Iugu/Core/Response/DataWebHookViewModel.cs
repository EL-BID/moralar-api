using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class DataWebHookViewModel
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("data[id]")]
        public string DataId { get; set; }

        [JsonProperty("data[status]")]
        public string DataStatus { get; set; }

        [JsonProperty("data[account_id]")]
        public string DataAccountId { get; set; }

        [JsonProperty("data[payment_method]")]
        public string DataPaymentMethod { get; set; }
    }
}
