using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class ListIuguAdvanceResponse
    {
        [JsonProperty("totalItems")]
        public long TotalItems { get; set; }

        [JsonProperty("items")]
        public List<ItemAdvance> Items { get; set; }

        public ListIuguAdvanceResponse()
        {
            Items = new List<ItemAdvance>();
        }
    }
}
