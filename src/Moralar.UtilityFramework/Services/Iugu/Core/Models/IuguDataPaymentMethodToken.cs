using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;


namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguDataPaymentMethodToken
    {
        //
        // Resumen:
        //     Número do Cartão de Crédito
        [JsonProperty("number")]
        [Display(Name = "Número")]
        public string Number { get; set; }

        //
        // Resumen:
        //     CVV do Cartão de Crédito
        [JsonProperty("verification_value")]
        [Display(Name = "Código de verificação")]
        public string VerificationValue { get; set; }

        //
        // Resumen:
        //     Nome do Cliente como está no Cartão
        [JsonProperty("first_name")]
        [Display(Name = "Primeiro nome como está no cartão")]
        public string FirstName { get; set; }

        //
        // Resumen:
        //     Sobrenome do Cliente como está no Cartão
        [JsonProperty("last_name")]
        [Display(Name = "Sobrenome como está no cartão")]
        public string LastName { get; set; }

        //
        // Resumen:
        //     Mês de Vencimento no Formato MM (Ex: 01, 02, 12)
        [JsonProperty("month")]
        [Display(Name = "Mês de vencimento")]
        public string Month { get; set; }

        //
        // Resumen:
        //     Ano de Vencimento no Formato YYYY (2014, 2015, 2016)
        [JsonProperty("year")]
        [Display(Name = "Ano de vencimento")]
        public string Year { get; set; }
    }
}
