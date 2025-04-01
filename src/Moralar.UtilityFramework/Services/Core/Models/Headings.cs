using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Core.Models
{
    public class Headings
    {
        [JsonProperty("en")]
        public string En { get; set; }

        [JsonProperty("es")]
        public string Es { get; set; }
    }
}
