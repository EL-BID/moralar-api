using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguTriggerModel
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("data")]
        public DataTriggerModel Data { get; set; }
    }
}
