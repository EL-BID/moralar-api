using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.Shared
{
    public class LoginFamilyBirthdayViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public string HolderCpf { get; set; }

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        public long HolderBirthday { get; set; }
    }
}
