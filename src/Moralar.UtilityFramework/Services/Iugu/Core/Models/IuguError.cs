
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguError
    {
        [JsonProperty("errors")]
        public string Errors { get; set; }
    }
}
