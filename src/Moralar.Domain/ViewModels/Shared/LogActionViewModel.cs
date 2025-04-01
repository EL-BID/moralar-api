using Moralar.Data.Enum;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels
{
    public class LogActionViewModel : BaseViewModel
    {
        public long Created { get; set; }
        public LocalAction LocalAction { get; set; }
        public string LocalActionStr { get; set; }
        public TypeAction TypeAction { get; set; }
        public string TypeActionStr { get; set; }
        public TypeResposible TypeResposible { get; set; }
        public string TypeResposibleStr { get; set; }
        public string Message { get; set; }
        public string ReferenceId { get; set; }
        public string ResponsibleId { get; set; }
        public string ResponsibleName { get; set; }
        public string Justification { get; set; }
        public string MessageEx { get; set; }
        public string StackTrace { get; set; }
    }
}