
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguErrorsArray
    {
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }
    }
}
