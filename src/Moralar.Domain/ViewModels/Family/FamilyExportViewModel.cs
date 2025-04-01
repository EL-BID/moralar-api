using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyExportViewModel
    {
        /// <summary>
        /// Número do cadastro
        /// </summary>
        [Display(Name = "Número do cadastro")]
        public string Number { get; set; }
        /// <summary>
        /// Nome do morador titular
        /// </summary>
        [Display(Name = "Nome do titular")]
        public string Name { get; set; }
        /// <summary>
        /// CPF do morador titular
        /// </summary>
        [Display(Name = "CPF do titular")]
        public string Cpf { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; }


    }
}
