using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class InformativeRepository : BusinessBaseAsync<Informative>, IInformativeRepository
    {
        public InformativeRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}