using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Quiz;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.QuestionAnswer
{
    //QuestionAnswerViewModel
    public class QuestionAnswerRegisterViewModel : QuestionAnswerResponseViewModel
    {
        [JsonIgnore]
        
        [Display(Name = "Insira o nome do titular")]
        public string FamilyHolderName { get; set; }

        [JsonIgnore]
        [Display(Name = "Insira o código da Família")]
        public string FamilyHolderCpf { get; set; }

        [JsonIgnore]
        public string FamilyNumber { get; set; }

        [JsonIgnore]
        public string ResponsibleForResponsesName { get; set; }
        [JsonIgnore]
        public string ResponsibleForResponsesCpf { get; set; }
    }
}
