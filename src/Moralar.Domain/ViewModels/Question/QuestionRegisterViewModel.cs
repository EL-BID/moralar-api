using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.Question
{
    public class QuestionRegisterViewModel
    {
        [JsonIgnore]
        public string QuizId { get; set; }

        public List<QuestionViewModel> Question { get; set; }
    }
}
