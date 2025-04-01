using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class Item
    {
        //
        // Resumen:
        //     ID do ítem
        [JsonProperty("id")]
        public string ID { get; set; }

        //
        // Resumen:
        //     Descrição do ítem
        [JsonProperty("description")]
        public string Description { get; set; }

        //
        // Resumen:
        //     Preço em Centavos. Valores negativos entram como desconto no total da Fatura
        [JsonProperty("price_cents")]
        public int PriceCents { get; set; }

        //
        // Resumen:
        //     Quantidade
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        //
        // Resumen:
        //     Data de criação
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        //
        // Resumen:
        //     Data de modificação
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        //
        // Resumen:
        //     Preço
        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("_destroy")]
        public bool? Destroy { get; set; }

        public Item()
        {
            PriceCents = 100;
        }
    }
}
