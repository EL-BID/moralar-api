using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class BankRepository : BusinessBaseAsync<Bank>, IBankRepository
    {
        public BankRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}