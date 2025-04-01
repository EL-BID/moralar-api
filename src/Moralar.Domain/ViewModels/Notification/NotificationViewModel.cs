using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Notification
{
    public class NotificationViewModel : BaseViewModel
    {

        [IsReadOnly]
        /// <summary>
        /// Data do cadastro
        /// </summary>
        [Display(Name = "Data do cadastro")]
        public long? Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Título da notificação")]
        public string Title { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Descrição da notificação")]
        public string Description { get; set; }
        [Display(Name = "Imagem")]
        public string Image { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Enviar para todas as famílias?")]
        public bool AllFamily { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Id da Família")]
        public List<string> FamilyId { get; set; }
        public long? DateViewed { get; set; }
        public string? Module { get; set; }

        public string? SeenBy { get; set; }
    }
}
