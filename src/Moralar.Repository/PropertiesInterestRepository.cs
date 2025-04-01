using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class PropertiesInterestRepository : BusinessBaseAsync<PropertiesInterest>, IPropertiesInterestRepository
    {
        public PropertiesInterestRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}