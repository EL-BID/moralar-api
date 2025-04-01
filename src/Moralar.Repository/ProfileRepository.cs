using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class ProfileRepository : BusinessBaseAsync<Profile>, IProfileRepository
    {
        public ProfileRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}