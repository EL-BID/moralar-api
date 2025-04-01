using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.InformativeSended
{
    public class InformativeSendedViewModel : BaseViewModel
    {
        public string InformativeId { get; set; }        
        public string DatePublish { get; set; }
        public string Image { get; set; }       
        public string Description { get; set; }
        public long? DateViewed { get; set; }
    }
}
