using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.ResidencialProperty
{
    public class ResidencialPropertyExportViewModel
    {


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Código do imóvel")]

        public string Code { get; set; }
        /// <summary>
        /// Fotos
        /// </summary>
        [Display(Name = "Fotos")]
        public string Photo { get; set; }
        /// <summary>
        /// Planta
        /// </summary>
        [Display(Name = "Planta")]
        public string Project { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "CEP")]

        public string CEP { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Endereço")]

        public string StreetAddress { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número")]

        public string Number { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome da Cidade")]

        public string CityName { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome do Estado")]

        public string StateName { get; set; }
        
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "UF")]

        public string StateUf { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Bairro")]

        public string Neighborhood { get; set; }

        [Display(Name = "Complemento")]

        public string Complement { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Valor do imóvel")]

        public string PropertyValue { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo do imóvel")]

        public string TypeProperty { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Metragem quadrada")]

        public string SquareFootage { get; set; }

        [Display(Name = "Valor do condomínio")]

        public string CondominiumValue { get; set; }

        [Display(Name = "Valor do IPTU")]

        public string IptuValue { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número de pavimentos")]

        public string NumberFloors { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Andar de localização")]

        public string FloorLocation { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem Elevador?")]

        public string HasElavator { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número de quartos")]

        public string NumberOfBedrooms { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número de banheiros")]

        public string NumberOfBathrooms { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem área de serviço?")]

        public string HasServiceArea { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem garagem de serviço?")]

        public string HasGarage { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem quintal?")]

        public string HasYard { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem cisterna?")]

        public string HasCistern { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "É toda murada?")]

        public string HasWall { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem escada de acesso?")]

        public string HasAccessLadder { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tem rampa de acesso?")]

        public string HasAccessRamp { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "É Adaptada ou permite adaptação para PCD?")]

        public string HasAdaptedToPcd { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Regularização do imóvel")]

        public string PropertyRegularization { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Tipo de instalação de gás")]

        public string TypeGasInstallation { get; set; }


        /// <summary>
        /// Status
        /// </summary>
        [Display(Name = "Status")]
        public string TypeStatusResidencialProperty { get; set; }
        /// <summary>
        /// Situação
        /// </summary>
        [Display(Name = "Situação")]
        public string Blocked { get; set; }

    }
}
