using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class SubscriptionSubitem
    {
        //
        // Resumen:
        //     ID
        [JsonProperty("id")]
        public string ID { get; set; }

        //
        // Resumen:
        //     Descrição
        [JsonProperty("description")]
        public string Description { get; set; }

        //
        // Resumen:
        //     Quantidade
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        //
        // Resumen:
        //     Valor em centavos
        [JsonProperty("price_cents")]
        public int PriceCents { get; set; }

        //
        // Resumen:
        //     Preço
        [JsonProperty("price")]
        public string Price { get; set; }

        //
        // Resumen:
        //     Total
        [JsonProperty("total")]
        public string Total { get; set; }

        //
        // Resumen:
        //     Recorrente (Sim/Não)
        [JsonProperty("recurrent")]
        public bool? Recurrent { get; set; }
    }
}
