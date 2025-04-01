using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class UserAdministratorRepository : BusinessBaseAsync<UserAdministrator>, IUserAdministratorRepository
    {
        public UserAdministratorRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}