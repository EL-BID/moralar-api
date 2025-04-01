using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels
{
    public class CreditCardViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [MinLength(14, ErrorMessage = DefaultMessages.Minlength)]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número do cartão")]
        public string Number { get; set; }
        [Range(1, 12, ErrorMessage = DefaultMessages.Range)]
        [Display(Name = "Mês de vencimento")]
        public int ExpMonth { get; set; }
        [MinLength(3, ErrorMessage = DefaultMessages.Minlength)]
        [MaxLength(4, ErrorMessage = DefaultMessages.Maxlength)]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [JsonConverter(typeof(OnlyNumber))]
        [Display(Name = "Código de segurança")]
        public string Cvv { get; set; }
        [Display(Name = "Ano de vencimento")]
        public int ExpYear { get; set; }
        public string Flag { get; set; }
        public string Brand { get; set; }

    }
}