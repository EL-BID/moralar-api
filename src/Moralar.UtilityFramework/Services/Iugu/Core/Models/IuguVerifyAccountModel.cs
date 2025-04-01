
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguVerifyAccountModel : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("data")]
        public IuguAccountDataVerificationModel Data { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        public bool AlreadyVerified { get; set; }
    }
}
