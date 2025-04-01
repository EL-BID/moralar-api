using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class StateRepository : BusinessBaseAsync<Data.Entities.State>, IStateRepository
    {
        public StateRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}