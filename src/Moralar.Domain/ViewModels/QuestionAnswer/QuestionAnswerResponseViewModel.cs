using Moralar.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.QuestionAnswer
{
    public class QuestionAnswerResponseViewModel : BaseViewModel
    {
        public QuestionAnswerResponseViewModel()
        {
            Questions = new List<QuestionAnswerAuxViewModel>();
        }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Insira o código da Família")]
        public string FamilyId { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Insira o código do questionário")]
        public string QuizId { get; set; }
        public List<QuestionAnswerAuxViewModel> Questions { get; set; }
        public string ResponsibleForResponsesId { get; set; }


    }
}
