using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.Data.Enum;

namespace Moralar.Domain.ViewModels.ResidencialProperty
{
    public class ResidencialPropertyFilterViewModel
    {
        //[Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo do imóvel")]
        public TypeProperty? TypeProperty { get; set; }

        public double StartSquareFootage { get; set; }
        public double EndSquareFootage { get; set; }
        
        public double StartCondominiumValue { get; set; }
        public double EndCondominiumValue { get; set; }
        
        public double StartIptuValue { get; set; }
        public double EndIptuValue { get; set; }
        
        public int StartNumberOfBedrooms { get; set; }
        public int EndNumberOfBedrooms { get; set; }
        //[Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem garagem de serviço?")]
        public bool? HasGarage { get; set; }


        //[Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem escada de acesso?")]
        public bool? HasAccessLadder { get; set; }

        //[Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem rampa de acesso?")]
        public bool? HasAccessRamp { get; set; }

        //[Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "É Adaptada ou permite adaptação para PCD?")]
        public bool? HasAdaptedToPcd { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }

    }
}
