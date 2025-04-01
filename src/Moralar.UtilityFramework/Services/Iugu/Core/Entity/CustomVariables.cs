using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class CustomVariables
    {
        //
        // Resumen:
        //     Nome do atributo
        [JsonProperty("name")]
        public string Name { get; set; }

        //
        // Resumen:
        //     Valor do atributo
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
