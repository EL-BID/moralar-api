using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class NotificationSendedRepository : BusinessBaseAsync<NotificationSended>, INotificationSendedRepository
    {
        public NotificationSendedRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}