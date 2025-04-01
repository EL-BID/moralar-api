using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguChargeResponse : IuguBaseErrors
    {
        //
        // Resumen:
        //     Url do boleto
        public string Url { get; set; }

        //
        // Resumen:
        //     Informa se a cobrança foi gerada com sucesso
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("pdf")]
        public string Pdf { get; set; }

        [JsonProperty("identification")]
        public object Identification { get; set; }

        //
        // Resumen:
        //     Número da fatura da cobrança
        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

        //
        // Resumen:
        //     Mensagem de resposta
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("LR")]
        public string LR { get; set; }

        public string MsgLR { get; set; }
    }
}
