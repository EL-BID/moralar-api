using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Moralar.Domain.ViewModels
{
    public class LogActionExportViewModel
    {
        /// <summary>
        /// Data e hora
        /// </summary>
        [Display(Name = "Data e hora")]
        public string Created { get; set; }
        /// <summary>
        /// Local
        /// </summary>
        /// <example></example>
        [Display(Name = "Local")]
        public string LocalAction { get; set; }
        [Display(Name = "Tipo de ação")]
        public string TypeAction { get; set; }
        [Display(Name = "Tipo de responsavél")]
        public string TypeResposible { get; set; }
        [Display(Name = "Mensagem")]
        public string Message { get; set; }

        [Display(Name = "Responsável")]
        public string ResponsibleName { get; set; }
        [Display(Name = "Justificativa")]
        public string Justification { get; set; }

        /// <summary>
        /// Referencia do Item
        /// </summary>
        [Display(Name = "Item alterado")]
        public string ReferenceId { get; set; }
    }
}