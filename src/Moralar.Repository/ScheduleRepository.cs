using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class ScheduleRepository : BusinessBaseAsync<Data.Entities.Schedule>, IScheduleRepository
    {
        public ScheduleRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}