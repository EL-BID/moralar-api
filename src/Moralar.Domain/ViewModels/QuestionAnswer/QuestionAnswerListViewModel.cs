using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.QuestionAnswer
{
    public class QuestionAnswerListViewModel
    {
        public QuestionAnswerListViewModel()
        {
            Answers = new List<string>();
           
        }
        public string QuizId { get; set; }        
        public  string Title { get; set; }
        public long Date { get; set; }
        public string FamilyNumber { get; set; }
        public string FamilyHolderName { get; set; }
        public string FamilyHolderCpf { get; set; }

        public string QuestionId { get; set; }
        public string Question { get; set; }
        public TypeResponse TypeResponse { get; set; }
        public List<string> Answers { get; set; }
       

    }
}
