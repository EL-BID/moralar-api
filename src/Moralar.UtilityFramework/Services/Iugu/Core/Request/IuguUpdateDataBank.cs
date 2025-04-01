
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Request
{
    public class IuguUpdateDataBank : IuguBaseErrors
    {
        //
        // Resumen:
        //     CODIGO DO BANCO
        [JsonProperty("bank")]
        [Display(Name = "Banco")]
        public string Bank { get; set; }

        //
        // Resumen:
        //     TIPO DE CONTA CC / CP
        [JsonProperty("account_type")]
        [Display(Name = "Tipo de conta")]
        public string AccountType { get; set; }

        //
        // Resumen:
        //     Conta
        [JsonProperty("account")]
        [Display(Name = "Conta")]
        public string Account { get; set; }

        //
        // Resumen:
        //     Agência
        [JsonProperty("agency")]
        [Display(Name = "Agência")]
        public string Agency { get; set; }

        //
        // Resumen:
        //     Validação automática
        [JsonProperty("automatic_validation")]
        [Display(Name = "Validação automática")]
        public string AutomaticValidation { get; set; }
    }
}
