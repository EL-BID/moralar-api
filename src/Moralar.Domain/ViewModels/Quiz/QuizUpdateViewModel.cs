using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizUpdateViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public TypeQuiz TypeQuiz { get; set; }
        //public List<QuestionViewModel> Question { get; set; }
        public QuestionRegisterViewModel QuestionRegister { get; set; }
    }
}
