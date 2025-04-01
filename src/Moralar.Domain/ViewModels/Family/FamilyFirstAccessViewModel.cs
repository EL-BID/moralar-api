using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyFirstAccessViewModel
    {
        /// <summary>
        /// CPF
        /// </summary>
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [IsValidCpf(ErrorMessage = DefaultMessages.CpfInvalid)]
        [JsonConverter(typeof(OnlyNumber))]
        public string Cpf { get; set; }
        /// <summary>
        /// Senha
        /// </summary>
        [Display(Name = "Senha")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Password { get; set; }
        /// <summary>
        /// Nome da mãe
        /// </summary>
        [Display(Name = "Primeiro nome da sua mãe")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string MotherName { get; set; }
        /// <summary>
        /// Local de nascimento
        /// </summary>
        [Display(Name = "Cidade onde nasceu")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string MotherCityBorned { get; set; }

    }
}
