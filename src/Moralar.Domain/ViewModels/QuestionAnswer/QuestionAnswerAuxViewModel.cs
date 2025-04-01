using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.QuestionAnswer
{
    public class QuestionAnswerAuxViewModel
    {
        [Display(Name = "Insira o código da descrição da resposta")]
        public List<string> QuestionDescriptionId { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Código da questão")]
        public string QuestionId { get; set; }

        [Display(Name = "Insira a resposta descritiva da questão")]
        public string AnswerDescription { get; set; }
    }
}
