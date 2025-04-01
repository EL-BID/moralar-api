using System.ComponentModel.DataAnnotations;

using Moralar.Data.Enum;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyHolderExportViewModel
    {

        [Display(Name = "Número do cadastro")]
        public string Number { get; set; }

        [Display(Name = "Nome do titular")]
        public string Name { get; set; }

        [Display(Name = "CPF do titular")]
        public string Cpf { get; set; }

        [Display(Name = "Status da linha do tempo")]
        public string TypeScheduleStatus { get; set; }

        [Display(Name = "Assunto")]
        public string TypeSubject { get; set; }
    }
}
