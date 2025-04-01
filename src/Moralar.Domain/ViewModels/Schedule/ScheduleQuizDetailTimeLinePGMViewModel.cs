using System;
using Moralar.Data.Enum;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleQuizDetailTimeLinePGMViewModel
    {
        public string Id { get; set; }
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string FamilyId { get; set; }
        public string QuizId { get; set; }
        public string QuizFamilyId { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string HasAnswered { get; set; }
        public TypeStatus TypeStatus { get; set; }
        public TypeQuiz TypeQuiz { get; set; }

    }
}
