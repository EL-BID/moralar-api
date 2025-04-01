using Newtonsoft.Json;

namespace Moralar.Domain.ViewModels.Firebase
{
    public class MessageFireBaseViewModel
    {
        [JsonProperty("messages")]
        public string Message { get; set; }
        [JsonProperty("serderId")]
        public string SenderId { get; set; }
        [JsonProperty("dataSend")]
        public long DataSend { get; set; }
        [JsonProperty("read")]
        public bool Read { get; set; }
    }
}