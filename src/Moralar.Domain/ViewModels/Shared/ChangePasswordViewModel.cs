using System.ComponentModel.DataAnnotations;

namespace Moralar.Domain.ViewModels
{
    public class ChangePasswordViewModel
    {

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Senha atual")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nova senha")]
        public string NewPassword { get; set; }
    }
}