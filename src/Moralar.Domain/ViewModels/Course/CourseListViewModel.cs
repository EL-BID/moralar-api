using System;
using System.Collections.Generic;
using System.Text;
using Moralar.Data.Enum;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Course
{
    public class CourseListViewModel : BaseViewModel
    {
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string Title { get; set; }




        public string Img { get; set; }



        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }




        public string Schedule { get; set; }




        public string Place { get; set; }




        public string WorkLoad { get; set; }




        public string Description { get; set; }




        public int StartTargetAudienceAge { get; set; }



        public int EndTargetAudienceAge { get; set; }


        public TypeGenre TypeGenre { get; set; }



        public int NumberOfVacancies { get; set; }

        public int TotalInscriptions { get; set; }

        public int TotalWaitingList { get; set; }
        public bool Blocked { get; set; }
        public string Name { get; set; }
    }
}
