using System;
using System.Collections.Generic;
using System.Text;
using Moralar.Data.Enum;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.ScheduleHistory
{
    public class ScheduleHistoryViewModel : BaseViewModel
    {
        public ScheduleHistoryViewModel()
        {
            Children = new List<ScheduleHistoryViewModel>();
        }
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public long Date { get; set; }
        public string ParentId { get; set; }
        public string ScheduleId { get; set; }
        public TypeSubject TypeSubject { get; set; }
        public TypeScheduleStatus TypeScheduleStatus { get; set; }
        public List<ScheduleHistoryViewModel> Children { get; set; }
    }
}
