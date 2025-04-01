using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class LogActionRepository : BusinessBaseAsync<LogAction>, ILogActionRepository
    {
        public LogActionRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}