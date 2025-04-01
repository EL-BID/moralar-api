using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleRegisterViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Data do Agendamento")]
        public long Date { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Local")]
        public string Place { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Identificador da família")]
        public string FamilyId { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Assunto")]
        public TypeSubject TypeSubject { get; set; }
        [JsonIgnore]
        public TypeScheduleStatus TypeScheduleStatus { get; set; }
        public QuizAux Quiz { get; set; }


    }
}
