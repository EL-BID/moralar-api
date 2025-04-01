using System.ComponentModel.DataAnnotations;

using Moralar.Data.Enum;

using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyHolderListViewModel : BaseViewModel
    {
        public long Created { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número do cadastro")]
        public string Number { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome do titular")]
        public string Name { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "CPF do titular")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Data de nascimento")]
        public long? Birthday { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Gênero")]
        public TypeGenre? Genre { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Telefone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Escolaridade")]
        public TypeScholarity? Scholarity { get; set; }

        public TypeSubject TypeSubject { get; set; }
        public TypeScheduleStatus TypeScheduleStatus { get; set; }
        public string ScheduleId { get; set; }

        public bool Blocked { get; set; }
    }
}
