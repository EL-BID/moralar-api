using System.ComponentModel.DataAnnotations;

using Moralar.Data.Enum;

namespace Moralar.Domain.ViewModels.Property
{
    public class ResidencialPropertyFeatureViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Valor do imóvel")]
        public double PropertyValue { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo do imóvel")]
        public TypeProperty TypeProperty { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Metragem quadrada")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335",ErrorMessage ="Insira um número maior que 0")]

        public decimal SquareFootage { get; set; }


        [Display(Name = "Valor do condomínio")]
        public double CondominiumValue { get; set; }


        [Display(Name = "Valor do IPTU")]
        public double IptuValue { get; set; }


        
        [Display(Name = "Bairro de localização")]
        public string Neighborhood { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número de pavimentos")]
        [Range(0, 20, ErrorMessage = "Insira um valor de 0 até 20")]
        public int NumberFloors { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Andar de localização")]
        [Range(0, 20, ErrorMessage = "Insira um valor de 0 até 20")]
        public string FloorLocation { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem Elevador?")]
        public bool HasElavator { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número de quartos")]
        [Range(0, 2000, ErrorMessage = "Insira um valor maior que 0 ")]
        public int NumberOfBedrooms { get; set; }

        
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número de banheiros")]
        [Range(0, 2000, ErrorMessage = "Insira um valor maior que 0 ")]
        public int NumberOfBathrooms { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem área de serviço?")]
        public bool HasServiceArea { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem garagem de serviço?")]
        public bool HasGarage { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem quintal?")]
        public bool HasYard { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem cisterna?")]
        public bool HasCistern { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "É toda murada?")]
        public bool HasWall { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem escada de acesso?")]
        public bool HasAccessLadder { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem rampa de acesso?")]
        public bool HasAccessRamp { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "É Adaptada ou permite adaptação para PCD?")]
        public bool HasAdaptedToPcd { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Regularização do imóvel")]
        public TypePropertyRegularization PropertyRegularization { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo de instalação de gás")]
        public TypePropertyGasInstallation TypeGasInstallation { get; set; }
        /// <summary>
        /// Beira de rua?
        /// </summary>
        [Display(Name = "Beira de rua?")]
        public bool StreetEdge { get; set; }
    }
}
