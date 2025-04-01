using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.Property
{
    public class ResidencialPropertyAdressViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Endereço")]
        public string StreetAddress { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Número do endereço")]
        public string Number { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome da Cidade")]
        public string CityName { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id da Cidade")]
        public string CityId { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome do Estado")]
        public string StateName { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "UF")]
        public string StateUf { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id do Estado")]
        public string StateId { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Bairro")]
        public string Neighborhood { get; set; }


       
        [Display(Name = "Complemento")]
        public string Complement { get; set; }


       
        [Display(Name = "Localização")]
        public string Location { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Latitude")]
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Longitude")]
        public double Longitude { get; set; }
    }
}
