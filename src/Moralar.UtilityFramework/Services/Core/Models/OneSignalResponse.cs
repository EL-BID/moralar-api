
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Core.Models
{
    public class OneSignalResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("recipients")]
        public int Recipients { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("warnings")]
        public List<string> Warnings { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        public bool Erro { get; set; }

        public int StatusCode { get; set; }
    }
}
