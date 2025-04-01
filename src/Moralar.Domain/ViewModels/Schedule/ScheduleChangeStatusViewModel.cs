using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleChangeStatusViewModel:BaseViewModel
    {
        public string Place { get; set; }
        public string Description { get; set; }
        public long Date { get; set; }
        public string FamilyId { get; set; }
        //public string ScheduleId { get; set; }
        public TypeScheduleStatus TypeScheduleStatus { get; set; }
    }
}
