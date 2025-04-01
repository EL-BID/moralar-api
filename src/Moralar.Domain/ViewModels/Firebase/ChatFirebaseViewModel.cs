using Newtonsoft.Json;
using System.Collections.Generic;

namespace Moralar.Domain.ViewModels.Firebase
{
    public class ChatFirebaseViewModel
    {
        [JsonProperty("profile")]
        public ProfileChatFireBaseViewModel Profile { get; set; }
        [JsonProperty("driver")]

        public ProfileChatFireBaseViewModel Driver { get; set; }
        [JsonProperty("messages")]
        public Dictionary<string, MessageFireBaseViewModel> Messages { get; set; }
    }
}