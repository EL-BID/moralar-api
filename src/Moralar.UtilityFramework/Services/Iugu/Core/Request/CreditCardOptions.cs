
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class CreditCardOptions
    {
        //
        // Resumen:
        //     Ativo
        [JsonProperty("active")]
        public bool Active { get; set; }

        //
        // Resumen:
        //     Descrição que apareça na Fatura do Cartão do Cliente (Máx: 12 Caractéres)
        [JsonProperty("soft_descriptor")]
        public string SoftDescriptor { get; set; }

        //
        // Resumen:
        //     Ativar parcelamento
        [JsonProperty("installments")]
        public bool Installments { get; set; }

        //
        // Resumen:
        //     Repasse de Juros de Parcelamento ativo? true ou false
        [JsonProperty("installments_pass_interest")]
        public bool InstallmentsPassInterest { get; set; }

        //
        // Resumen:
        //     Número máximo de parcelas (Nr entre 1 a 12)
        [JsonProperty("max_installments")]
        public int? MaxInstallments { get; set; }

        //
        // Resumen:
        //     Número de parcelas sem cobrança de juros ao cliente (Nr entre 1 a 12)
        [JsonProperty("max_installments_without_interest")]
        public int? MaxInstallmentsWithoutInterest { get; set; }

        //
        // Resumen:
        //     Habilita o fluxo de pagamento em duas etapas (Autorização e Captura)
        [JsonProperty("two_step_transaction")]
        public bool TwoStepTransaction { get; set; }
    }
}
