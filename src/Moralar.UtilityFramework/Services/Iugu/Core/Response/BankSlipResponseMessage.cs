using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class BankSlipResponseMessage
    {
        [JsonProperty("digitable_line")]
        public string DigitableLine { get; set; }

        [JsonProperty("barcode_data")]
        public string BarcodeData { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }
    }
}