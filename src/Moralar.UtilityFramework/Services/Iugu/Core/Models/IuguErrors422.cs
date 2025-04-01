using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguErrors422
    {
        [JsonProperty("errors")]
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
