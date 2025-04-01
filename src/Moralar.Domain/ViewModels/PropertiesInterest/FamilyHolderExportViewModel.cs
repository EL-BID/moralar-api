using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Moralar.Data.Enum;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.PropertiesInterest
{
    public class PropertiesInterestExportViewModel
    {
        [Display(Name = "Número do cadastro")]
        public string HolderNumber { get; set; }

        [Display(Name = "Nome do titular")]
        public string HolderName { get; set; }

        [Display(Name = "Email do titular")]
        public string HolderEmail { get; set; }

        [Display(Name = "CPF do titular")]
        public string HolderCpf { get; set; }

        [Display(Name = "Código da residência")]
        public string ResidencialCode { get; set; }
 
        [Display(Name = "Interessados")]
        public int Interest { get; set; }
        //public ResidencialPropertyAdress ResidencialPropertyAdress { get; set; }
        //public List<PriorityRate> PriorityRates { get; set; }
    }
}
