using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.ResidencialProperty;
using Newtonsoft.Json;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Property
{
    public class ResidencialPropertyViewModel : BaseViewModel
    {
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Código do imóvel")]
        public string Code { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Foto")]
        [LimitElements(1, 15, ErrorMessage = "Selecione no mínimo 1 e no máximo 15 imagens")]
        [IsReadOnly]
        public List<ResidencialPropertyPhoto> Photo { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Planta")]
        [JsonConverter(typeof(RemovePathImage))]
        public string Project { get; set; }
        public string Description { get; set; }
        public bool Blocked { get; set; }
        public TypeStatusResidencial TypeStatusResidencialProperty { get; set; }
        [IsReadOnly]
        public ResidencialPropertyAdressViewModel ResidencialPropertyAdress { get; set; }
        [IsReadOnly]
        public ResidencialPropertyFeatureViewModel ResidencialPropertyFeatures { get; set; }
        /// <summary>
        /// Total de famílias interessadas
        /// </summary>
        [Display(Name = "Famílias interessadas")]
        public long InterestedFamilies { get; set; }
        public double? Distance { get; set; }

    }
}
