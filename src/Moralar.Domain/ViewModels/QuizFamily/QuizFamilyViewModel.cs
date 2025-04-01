using Moralar.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizFamilyViewModel: BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id do Questionário")]
        public string QuizId { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Icluir todas as famílias?")]
        public bool AllFamily { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id da Família")]
        public List<string> FamilyId { get; set; }
    }
}
