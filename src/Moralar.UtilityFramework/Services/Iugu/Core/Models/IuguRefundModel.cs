
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguRefundModel
    {
        [JsonProperty("partial_value_refund_cents")]
        public int PartialValueRefundCents { get; set; }
    }
}
