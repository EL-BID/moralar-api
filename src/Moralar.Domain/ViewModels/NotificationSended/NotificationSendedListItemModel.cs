using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moralar.Domain.ViewModels.NotificationSended
{
    public class NotificationSendedListItemModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long Created { get; set; }
    }
}