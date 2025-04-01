using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizExportViewModel
    {
        /// <summary>
        /// Identificador
        /// </summary>
        [Display(Name = "Identificador")]
        public string Id { get; set; }

        [Display(Name = "Título")]
        public string Title { get; set; }

        [Display(Name = "Data do Cadastro")]
        public string Created { get; set; }

        /// <summary>
        /// Total de respostas
        /// </summary>
        [Display(Name = "Total de respostas")]
        public long TotalAnswers { get; set; }
    }
}
