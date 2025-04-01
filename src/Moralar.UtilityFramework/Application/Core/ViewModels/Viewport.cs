
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class Viewport
    {
        [JsonProperty("northeast")]
        public Northeast Northeast { get; set; }

        [JsonProperty("southwest")]
        public Southwest Southwest { get; set; }
    }
}
