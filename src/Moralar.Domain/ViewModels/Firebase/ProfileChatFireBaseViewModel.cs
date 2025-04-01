using Newtonsoft.Json;

namespace Moralar.Domain.ViewModels.Firebase
{
    public class ProfileChatFireBaseViewModel
    {
        public ProfileChatFireBaseViewModel(string targertId)
        {
            TargetId = targertId;
        }

        [JsonProperty("typing")]
        public bool Typing { get; set; }

        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }

        [JsonProperty("noRead")]
        public int NoRead { get; set; }

        [JsonProperty("targetId")]
        public string TargetId { get; set; }
    }
}