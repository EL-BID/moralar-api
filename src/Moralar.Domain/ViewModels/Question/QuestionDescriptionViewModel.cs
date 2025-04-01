using Moralar.Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Question
{
    public class QuestionDescriptionViewModel : BaseViewModel
    {
        public string Description { get; set; }
        [JsonIgnore]
        public string QuestionId { get; set; }
    }
}
