using Newtonsoft.Json;


namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguTransferModel : IuguBaseErrors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("amount_cents")]
        public string AmountCents { get; set; }

        [JsonProperty("amount_localized")]
        public string AmountLocalized { get; set; }

        [JsonProperty("receiver")]
        public IuguReceiver Receiver { get; set; }
    }
}
