using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Profile
{
    public class ProfileImportViewModel
    {
        /// <summary>
        /// Nome completo
        /// </summary>
        /// <example></example>
        [Display(Name = "Nome completo*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(1)]
        public string Name { get; set; }

        [Display(Name = "Cargo*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(2)]

        public string JobPost { get; set; }
        /// <summary>
        /// CPF 
        /// </summary>
        [Display(Name = "CPF*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [IsValidCpf(ErrorMessage = DefaultMessages.CpfInvalid)]
        [JsonConverter(typeof(OnlyNumber))]
        [Column(3)]
        public string Cpf { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        [Display(Name = "E-mail*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [EmailAddress(ErrorMessage = DefaultMessages.EmailInvalid)]
        [JsonConverter(typeof(ToLowerCase))]
        [Column(4)]
        public string Email { get; set; }

        [Display(Name = "Telefone (ex: 550000000-0000)")]
        [JsonConverter(typeof(OnlyNumber))]
        [Column(5)]
        public string Phone { get; set; }

    }
}