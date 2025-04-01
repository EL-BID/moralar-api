using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels
{
    public class ValidationViewModel
    {

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [EmailAddress(ErrorMessage = DefaultMessages.EmailInvalid)]
        [JsonConverter(typeof(ToLowerCase))]
        public string Email { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [IsValidCpf(ErrorMessage = DefaultMessages.CpfInvalid)]
        [JsonConverter(typeof(OnlyNumber))]
        public string Cpf { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [IsValidCnpj(ErrorMessage = DefaultMessages.CnpjInvalid)]
        [JsonConverter(typeof(OnlyNumber))]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Login { get; set; }

    }
}