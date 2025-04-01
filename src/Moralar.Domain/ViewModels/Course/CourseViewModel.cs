using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.Data.Enum;
using Moralar.Domain.ViewModels.Family;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Course
{
    public class CourseViewModel : BaseViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Título")]
        public string Title { get; set; }



        [Display(Name = "Imagem")]
        public string Img { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Data de Início")]
        public long StartDate { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Data final")]
        public long EndDate { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Hora de início")]
        public string Schedule { get; set; }




        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Local do curso")]
        public string Place { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Carga horária")]
        public string WorkLoad { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Descrição do curso ")]
        public string Description { get; set; }



        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Idade mínima do público alvo")]
        public int StartTargetAudienceAge { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Idade máxima do público alvo")]
        public int EndTargetAudienceAge { get; set; }

        [Display(Name = "Gênero")]
        public TypeGenre? TypeGenre { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Quantidade de vagas")]
        public int NumberOfVacancies { get; set; }

        [Display(Name = "Família inscrita no curso?")]
        public bool IsSubscribed { get; set; }

        [Display(Name = "Total de inscritos no curso")]
        [IsReadOnly]
        public long TotalSubscribers { get; set; }

        [Display(Name = "Total de usuários aguardando na lista de espera")]
        [IsReadOnly]
        public long TotalWaitingList { get; set; }

        [Display(Name = "Lista com os dados do(s) usuário(s) inscritos")]
        [IsReadOnly]
        public List<FamilyCourseRegisteredViewModel> ListSubscribers { get; set; } = new List<FamilyCourseRegisteredViewModel>();
        
        [Display(Name = "Lista com os dados do(s) usuário(s) aguardando na lista de espera")]
        [IsReadOnly]
        public List<FamilyCourseWaitingViewModel> ListWaitingList { get; set; } = new List<FamilyCourseWaitingViewModel>();
    }
}
