using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Course
{
    public class CourseFamilyViewModel : BaseViewModel
    {
        public string FamilyId { get; set; }
        public string CourseId { get; set; }
        public bool WaitInTheQueue { get; set; }
    }
}
