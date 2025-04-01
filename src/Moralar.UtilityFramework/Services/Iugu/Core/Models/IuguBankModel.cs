using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguBankModel
    {
        [JsonProperty("bank")]
        public string Bank { get; set; }

        [JsonProperty("bank_cc")]
        public string BankCc { get; set; }

        [JsonProperty("bank_ag")]
        public string BankAg { get; set; }

        [JsonProperty("account_type")]
        public string AccountType { get; set; }
    }
}
