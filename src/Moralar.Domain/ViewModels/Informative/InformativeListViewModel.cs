using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Informative
{
    public class InformativeListViewModel : InformativeViewModel
    {
        public string Created { get; set; }
        public bool Status { get; set; }
        public bool Blocked { get; set; }
    }
}
