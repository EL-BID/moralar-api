using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class Logs
    {
        //
        // Resumen:
        //     Descrição da Entrada de Log
        [JsonProperty("description")]
        public string Description { get; set; }

        //
        // Resumen:
        //     Anotações da Entrada de Log
        [JsonProperty("notes")]
        public string Notes { get; set; }
    }
}
