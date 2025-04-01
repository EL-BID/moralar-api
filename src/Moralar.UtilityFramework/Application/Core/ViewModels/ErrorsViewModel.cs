
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class ErrorsViewModel
    {
        [JsonProperty("errors")]
        public string Errors { get; set; }
    }
}
