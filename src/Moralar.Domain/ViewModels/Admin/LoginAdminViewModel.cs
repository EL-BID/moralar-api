using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Admin
{
    public class LoginAdminViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]

        public string Password { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [EmailAddress(ErrorMessage = DefaultMessages.EmailInvalid)]
        [JsonConverter(typeof(ToLowerCase))]

        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}