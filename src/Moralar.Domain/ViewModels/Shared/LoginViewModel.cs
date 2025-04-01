using Moralar.Data.Enum;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels
{
    public class LoginViewModel
    {

        public string Cpf { get; set; }
        public string Login { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [EmailAddress(ErrorMessage = DefaultMessages.EmailInvalid)]
        [JsonConverter(typeof(ToLowerCase))]
        public string Email { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string ProviderId { get; set; }
        public TypeProvider TypeProvider { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public TypeUserProfile TypeUserProfile { get; set; }
    }
}