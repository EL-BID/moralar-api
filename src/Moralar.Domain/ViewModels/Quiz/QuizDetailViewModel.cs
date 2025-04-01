using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizDetailViewModel
    {
        public QuizDetailViewModel() {
            QuestionViewModel = new List<QuestionViewModel>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public TypeQuiz TypeQuiz { get; set; }
        public List<QuestionViewModel> QuestionViewModel { get; set; }
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}
