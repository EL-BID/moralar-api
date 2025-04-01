using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Profile
{
    public class ProfileUpdateViewModel : ProfileViewModel
    {
        [IsReadOnly]
        public string Password { get; set; }
    }
}
