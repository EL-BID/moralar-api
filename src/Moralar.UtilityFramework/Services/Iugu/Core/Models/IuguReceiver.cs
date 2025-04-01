using Newtonsoft.Json;


namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguReceiver
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
