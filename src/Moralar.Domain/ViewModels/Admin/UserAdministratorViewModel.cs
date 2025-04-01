using Moralar.Data.Enum;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Admin
{
    public class UserAdministratorViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Password { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string Name { get; set; }
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [JsonConverter(typeof(ToLowerCase))]
        public string Email { get; set; }
        public int Level { get; set; }
        public bool Blocked { get; set; }
        [JsonIgnore]
        public TypeUserProfile TypeProfile { get; set; }
    }
}