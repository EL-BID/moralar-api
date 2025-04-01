using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.NotificationSended
{
    public class NotificationSendedViewModel : BaseViewModel
    {
        public long? Created { get; set; }
        public string FamilyId { get; set; }
        public long DateViewed { get; set; }
    }
}