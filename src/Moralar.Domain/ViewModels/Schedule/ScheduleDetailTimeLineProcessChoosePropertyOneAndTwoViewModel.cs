using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Property;
using Moralar.Domain.ViewModels.ScheduleHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel
    {
        public ScheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel()
        {
            DetailQuiz = new List<ScheduleQuizDetailTimeLinePGMViewModel>();
            DetailEnquete = new List<ScheduleQuizDetailTimeLinePGMViewModel>();
            Courses = new List<ScheduleCourseViewModel>();
        }
        public long Date { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public string FamilyId { get; set; }
        public string HolderNumber { get; set; }
        public string HolderName { get; set; }
        public string HolderCpf { get; set; }

        public TypeSubject TypeSubject { get; set; }

        public TypeScheduleStatus TypeScheduleStatus { get; set; }
        public List<ScheduleQuizDetailTimeLinePGMViewModel> DetailQuiz { get; set; }
        public List<ScheduleQuizDetailTimeLinePGMViewModel> DetailEnquete { get; set; }
        public List<ScheduleCourseViewModel> Courses { get; set; }
        public List<ScheduleHistoryViewModel> Schedules { get; set; }
    }
}
