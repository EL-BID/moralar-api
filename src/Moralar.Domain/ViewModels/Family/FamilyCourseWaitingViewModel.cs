using System.ComponentModel.DataAnnotations;

using Moralar.Data.Enum;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyCourseWaitingViewModel
    {
        
        [Display(Name = "Número do cadastro")]
        public string Number { get; set; }

        
        [Display(Name = "Nome do titular")]
        public string Name { get; set; }

       
        [Display(Name = "CPF do titular")]
        public string Cpf { get; set; }

        [Display(Name = "Status")]    
        public TypeStatusCourse? TypeStatusCourse { get; set; }

    }
}
