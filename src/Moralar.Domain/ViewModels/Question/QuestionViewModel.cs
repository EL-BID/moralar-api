using Moralar.Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Question
{
    public class QuestionViewModel : BaseViewModel
    {
        public QuestionViewModel()
        {
            Description = new List<QuestionDescriptionViewModel>();
        }
        [JsonIgnore]
        [Display(Name = "Título do questionário")]
        public string NameTitle { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Pergunta")]
        public string NameQuestion { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo de resposta")]
        public TypeResponse TypeResponse { get; set; }



        [Display(Name = "Descrição da opção")]
        public List<QuestionDescriptionViewModel> Description { get; set; }
    }
}
