using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class EarlyPaymentDiscountsModel
    {
        //
        // Resumen:
        //     VALOR EM CENTAVOS DO DESCONTO
        [JsonProperty("value_cents")]
        public int? ValueCents { get; set; }

        //
        // Resumen:
        //     PORCENTAGEM DE DESCONTO
        [JsonProperty("percent")]
        public int? Percent { get; set; }

        //
        // Resumen:
        //     DIAS ANTES DO VENCIMENTOS
        [JsonProperty("days")]
        public int Days { get; set; }
    }
}
