using Moralar.Domain.ViewModels.Family;
using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.InformativeSended
{
    public class InformativeSendedDetailViewModel:BaseViewModel
    {
        public InformativeSendedDetailViewModel()
        {
            FamilyHolders = new List<FamilyHolderListViewModel>();
        }
        public long Created { get; set; }
        public string Description { get; set; }
        public List<FamilyHolderListViewModel> FamilyHolders { get; set; }
     
    }
}
