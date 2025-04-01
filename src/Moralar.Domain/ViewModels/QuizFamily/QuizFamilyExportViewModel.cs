using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.QuizFamily
{
    public class QuizFamilyExportViewModel
    {

        /// <summary>
        /// Título do questionário
        /// </summary>
        [Display(Name = "Título do questionário")]
        public string Title { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        [Display(Name = "Data")]
        public string Created { get; set; }
        /// <summary>
        /// Número do cadastro
        /// </summary>
        [Display(Name = "Número do cadastro")]
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
