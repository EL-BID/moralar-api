using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuestionAnswerViewModel : BaseViewModel
    {
        public List<string> QuestionDescriptionId { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Código da questão")]
        public string QuestionId { get; set; }

        [Display(Name = "Insira a resposta descritiva da questão")]
        public string AnswerDescription { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Insira o código da Família")]
        public string FamilyId { get; set; }

        public string ResponsibleForResponsesId { get; set; }
        
    }
}
