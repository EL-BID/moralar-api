using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class NotificationRepository : BusinessBaseAsync<Notification>, INotificationRepository
    {
        public NotificationRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}