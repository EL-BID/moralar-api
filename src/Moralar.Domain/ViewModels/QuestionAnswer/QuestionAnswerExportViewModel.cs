using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.QuestionAnswer
{
    public class QuestionAnswerExportViewModel
    {
        /// <summary>
        /// Identificador do Questionário
        /// </summary>
        [Display(Name = "Identificador do Questionário")]
        public string QuizId { get; set; }
        /// <summary>
        /// Titulo do questionário
        /// </summary>
        [Display(Name = "Titulo do questionário")]
        public string QuizTitle { get; set; }

        /// <summary>
        /// Nome do Titular
        /// </summary>
        [Display(Name = "Nome do Titular")]
        public string HolderName { get; set; }
        /// <summary>
        /// Cpf do titular
        /// </summary>
        [Display(Name = "Cpf do titular")]
        public string HolderCpf { get; set; }
        /// <summary>
        /// Identificador da questão
        /// </summary>
        [Display(Name = "Identificador da questão")]
        public string QuestionId { get; set; }

        /// <summary>
        /// Tipo de questão
        /// </summary>
        [Display(Name = "Tipo de questão")]
        public string TypeQuestion { get; set; }

        /// <summary>
        /// Questão
        /// </summary>
        [Display(Name = "Questão")]
        public string QuestionName { get; set; }
        /// <summary>
        /// Resposta
        /// </summary>
        [Display(Name = "Resposta")]
        public string Answers { get; set; }
    }
}