using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels
{
    public class DataBankViewModel : BaseViewModel

    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome do Responsável")]
        public string AccoutableName { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Cpf do Responsável")]

        public string AccoutableCpf { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired + " no formato ########-#")]
        [Display(Name = "Conta")]

        public string BankAccount { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired + " no formato ####-# digito é opcional")]
        [Display(Name = "Agencia")]

        public string BankAgency { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Banco")]

        public string Bank { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo de conta")]

        public string TypeAccount { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo de pessoa")]
        public string PersonType { get; set; }
        public string Cpnj { get; set; }
    }
}