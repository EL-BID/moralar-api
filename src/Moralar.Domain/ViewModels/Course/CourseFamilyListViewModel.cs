using Moralar.Data.Enum;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Course
{
    public class CourseFamilyListViewModel : BaseViewModel
    {
        
        [Display(Name = "Título")]
        public string Title { get; set; }   

        [Display(Name = "Data de Início")]
        public long StartDate { get; set; }
        
        [Display(Name = "Data final")]
        public long EndDate { get; set; }
       
        [Display(Name = "Hora de início")]
        public string Schedule { get; set; }

        [Display(Name = "Status")]
        public TypeStatusCourse TypeStatusCourse { get; set; }
        
    }
}
