using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleMapExportViewModel
    {
       
        /// <summary>
        /// Número de cadastro
        /// </summary>
        [Display(Name = "Número de cadastro")]
        public string HolderNumber { get; set; }
        /// <summary>
        /// Titular
        /// </summary>
        [Display(Name = "Titular")]
        public string HolderName { get; set; }
        /// <summary>
        /// CPF
        /// </summary>
        [Display(Name = "CPF")]
        public string HolderCpf { get; set; }
        /// <summary>
        /// Endereço de origem
        /// </summary>
        [Display(Name = "Endereço de origem")]
        public string AddressPropertyOrigin { get; set; }
        /// <summary>
        /// Endereço de destino
        /// </summary>
        [Display(Name = "Endereço de destino")]
        public string AddressPropertyDestination { get; set; }
        /// <summary>
        /// Distância em metros
        /// </summary>
        [Display(Name = "Distância em metros")]
        public string AddressPropertyDistanceMeters { get; set; }
        /// <summary>
        /// Distância em KM
        /// </summary>
        [Display(Name = "Distância em KM")]
        public string AddressPropertyDistanceKilometers { get; set; }
    }
}
