using Moralar.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizNewQuestionViewModel
    {
        public string Id { get; set; }
        public QuestionRegisterViewModel QuestionRegister { get; set; }
    }
}
