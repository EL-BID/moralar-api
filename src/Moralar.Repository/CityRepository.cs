using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class CityRepository : BusinessBaseAsync<Data.Entities.City>, ICityRepository
    {
        public CityRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}