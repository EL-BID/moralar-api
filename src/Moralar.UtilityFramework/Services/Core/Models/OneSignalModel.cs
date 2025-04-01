using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Core.Models
{
    public class OneSignalModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public bool Erro { get; set; }

        [JsonProperty("successful")]
        public int Successful { get; set; }

        [JsonProperty("failed")]
        public int Failed { get; set; }

        [JsonProperty("converted")]
        public int Converted { get; set; }

        [JsonProperty("remaining")]
        public int Remaining { get; set; }

        [JsonProperty("queued_at")]
        public int QueuedAt { get; set; }

        [JsonProperty("send_after")]
        public int SendAfter { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, string> Data { get; set; }

        [JsonProperty("canceled")]
        public bool Canceled { get; set; }

        [JsonProperty("headings")]
        public Headings Headings { get; set; }

        [JsonProperty("contents")]
        public Contents Contents { get; set; }
    }
}
