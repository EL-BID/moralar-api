using System;
using System.Collections.Generic;
using System.Text;
using Moralar.Domain.ViewModels.Family;
using Newtonsoft.Json;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.NotificationSended
{
    public class NotificationSendedListViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long Created { get; set; }
        public string NotificationId { get; set; }

        public string Image { get; set; }
        public List<string> FamilyId { get; set; } = new List<string>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FamilyHolderViewModel> Family { get; set; }
        public long? DateViewed { get; set; }
        public bool Arquived { get; set; }
    }
}
