using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;

using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Schedule
{
    public class ScheduleViewModel : BaseViewModel
    {
        public long Date { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public string FamilyId { get; set; }
        public string HolderNumber { get; set; }
        public string HolderName { get; set; }
        public string HolderCpf { get; set; }

        public TypeSubject TypeSubject { get; set; }

        public TypeScheduleStatus TypeScheduleStatus { get; set; }

        public QuizAux Quiz { get; set; }
    }
}