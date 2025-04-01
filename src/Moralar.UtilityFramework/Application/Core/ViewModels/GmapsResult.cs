
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core.ViewModels
{
    public class GmapsResult
    {
        [JsonProperty("results")]
        public List<Result> Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string ErroMessage { get; set; }
    }
}
