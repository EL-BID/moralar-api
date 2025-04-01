using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class AddressModel
    {
        //
        // Resumen:
        //     Rua
        [JsonProperty("street")]
        [Display(Name = "Rua")]
        public string Street { get; set; }

        //
        // Resumen:
        //     Número
        [JsonProperty("number")]
        [Display(Name = "Número")]
        public string Number { get; set; }

        //
        // Resumen:
        //     Cidade
        [JsonProperty("city")]
        [Display(Name = "Cidade")]
        public string City { get; set; }

        //
        // Resumen:
        //     Estado (Ex: SP)
        [JsonProperty("state")]
        [Display(Name = "Estado")]
        public string State { get; set; }

        //
        // Resumen:
        //     País
        [JsonProperty("country")]
        [Display(Name = "País")]
        public string Country { get; set; }

        //
        // Resumen:
        //     CEP
        [JsonProperty("zip_code")]
        [Display(Name = "Cep")]
        public string ZipCode { get; set; }
    }
}
