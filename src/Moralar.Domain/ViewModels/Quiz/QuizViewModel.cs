using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Question;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Quiz
{
    public class QuizViewModel : BaseViewModel
    {
        /// <summary>
        /// Data de cadastro
        /// </summary>
        [IsReadOnly]
        [Display(Name = "Data de cadastro")]
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        /// <summary>
        /// Título
        /// </summary>
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Título")]
        public string Title { get; set; }
        public TypeQuiz TypeQuiz { get; set; }
        public QuestionRegisterViewModel QuestionRegister { get; set; }
        public bool Blocked { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [Display(Name = "Status")]
        [IsReadOnly]
        public TypeStatus TypeStatus { get; set; }
    }
}
