using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Video
{
    public class VideoViewModel : BaseViewModel
    {
        public long Created { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public bool Blocked { get; set; }
        public string Name { get; set; }
    }
}
