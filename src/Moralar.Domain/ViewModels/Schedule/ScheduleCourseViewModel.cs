using System;

using Moralar.Data.Enum;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleCourseViewModel
    {
        public string Id { get; set; }
        public string CourseFamilyId { get; set; }
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string Title { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }       
        public string Schedule { get; set; }
        public TypeStatusCourse TypeStatusCourse { get; set; }
    }
}
