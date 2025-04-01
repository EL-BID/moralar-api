using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.VideoViewed
{
    public class VideoViewedViewModel : BaseViewModel
    {
        public string VideoId { get; set; }
        public string FamilyId { get; set; }
    }
}
