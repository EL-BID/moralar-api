using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleExportViewModel
    {
        /// <summary>
        /// Data e horário
        /// </summary>
        [Display(Name = "Data e horário")]
        public string Date { get; set; }
        /// <summary>
        /// Assunto
        /// </summary>
        [Display(Name = "Assunto")]
        public string TypeSubject { get; set; }
        /// <summary>
        /// Número de cadastro
        /// </summary>
        [Display(Name = "Número de cadastro")]
        public string HolderNumber { get; set; }
        /// <summary>
        /// Nome do morador titular
        /// </summary>
        [Display(Name = "Nome do morador titular")]
        public string HolderName { get; set; }
        /// <summary>
        /// CPF do morador titular
        /// </summary>
        [Display(Name = "CPF do morador titular")]
        public string HolderCpf { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
