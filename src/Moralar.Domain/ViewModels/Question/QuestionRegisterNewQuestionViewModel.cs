using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.Question
{
    public class QuestionRegisterNewQuestionViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Título do questionário")]
        public string NameTitle { get; set; }
        public QuestionViewModel Question { get; set; }
    }
}
