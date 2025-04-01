using System.ComponentModel.DataAnnotations;

using Moralar.Data.Enum;

using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizListViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Título")]
        public string Title { get; set; }
        public TypeQuiz TypeQuiz { get; set; }
        public TypeStatus TypeStatus { get; set; }
        public string FamilyId { get; set; }
        public long Created { get; set; }
        public long? DataBlocked { get; set; }
    }
}
