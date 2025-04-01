
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class Errors422ViewModel
    {
        [JsonProperty("errors")]
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
