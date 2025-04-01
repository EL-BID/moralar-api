using Moralar.Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyForgotPasswordViewModel
    {
        public string MotherName { get; set; }
        public string MotherCityBorned { get; set; }
        [JsonConverter(typeof(OnlyNumber))]
        public string Cpf { get; set; }
        public string Password { get; set; }

    }
}
