using Moralar.Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Property
{
    public class ResidencialPropertyChoicePropertyViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Código identificador do imóvel")]
        public string ResidencialPropertyId { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id da família")]
        public string FamiliIdResidencialChosen { get; set; }//mínimo 1 e máximo 15

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id da família")]
        public string FamiliIdResidencialContemplate { get; set; }//mínimo 1 e máximo 15
    }
}
