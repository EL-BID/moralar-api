
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class BankSlipOptions
    {
        //
        // Resumen:
        //     Ativo
        [JsonProperty("active")]
        public bool Active { get; set; }

        //
        // Resumen:
        //     Dias de Vencimento Extras no Boleto (Ex: 2)
        [JsonProperty("extra_due ")]
        public int? ExtraDueDays { get; set; }

        //
        // Resumen:
        //     Dias de Vencimento Extras na 2a Via do Boleto (Ex: 1)
        [JsonProperty("reprint_extra_due")]
        public bool ReprintExtraDueDays { get; set; }
    }
}
