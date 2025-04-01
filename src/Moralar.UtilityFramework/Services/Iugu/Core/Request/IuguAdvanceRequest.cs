using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class IuguAdvanceRequest : IuguBaseErrors
    {
        [JsonProperty("transactions")]
        public List<string> Transactions { get; set; }

        public IuguAdvanceRequest()
        {
            Transactions = new List<string>();
        }
    }
}
