using Moralar.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels.Family
{
    public class SearchViewModel
    {
        
        /// <summary>
        /// Termo de busca
        /// </summary>
        /// <example>Nome</example>
       // [Required(ErrorMessage = nameof(DefaultMessages.FieldRequired))]
        [Display(Name = nameof(DefaultMessages.FieldSearchTerm))]
        public string SearchTerm { get; set; }       
        public TypeSubject? TypeSubject { get; set; }

    }
}