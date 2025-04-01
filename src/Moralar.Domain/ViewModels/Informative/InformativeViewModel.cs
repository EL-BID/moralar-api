using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Informative
{
    public class InformativeViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Data do informativo")]
        public string DatePublish { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Descrição do informativo")]
        public string Description { get; set; }       

        //public List<string> FamilyId { get; set; }
    }
}
