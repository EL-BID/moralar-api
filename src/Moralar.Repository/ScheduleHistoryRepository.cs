using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class ScheduleHistoryRepository : BusinessBaseAsync<Data.Entities.ScheduleHistory>, IScheduleHistoryRepository
    {
        public ScheduleHistoryRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}