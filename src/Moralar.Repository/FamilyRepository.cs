using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class FamilyRepository : BusinessBaseAsync<Family>, IFamilyRepository
    {
        public FamilyRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}