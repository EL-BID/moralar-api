using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Core.Models
{
    public class Contents
    {
        [JsonProperty("en")]
        public string En { get; set; }

        [JsonProperty("es")]
        public string Es { get; set; }
    }
}
