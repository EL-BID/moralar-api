using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class AspNetLogViewModel
    {
        [JsonProperty("@t")]
        public DateTimeOffset Data { get; set; }

        [JsonProperty("@m")]
        public object Custom { get; set; }

        [JsonProperty("@i")]
        public string Identifier { get; set; }

        [JsonProperty("@l")]
        public string LogType { get; set; }

        [JsonProperty("SourceContext")]
        public string SourceContext { get; set; }

        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        [JsonProperty("RequestPath")]
        public string RequestPath { get; set; }
    }
}
