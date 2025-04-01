using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguAccountVerificationModel : IuguBaseErrors
    {
        [JsonProperty("data")]
        [IsClass]
        public IuguAccountDataVerificationModel Data { get; set; }

        [JsonProperty("automatic_validation")]
        public bool AutomaticValidation { get; set; }
    }
}
