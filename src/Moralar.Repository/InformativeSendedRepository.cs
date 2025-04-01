using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class InformativeSendedRepository : BusinessBaseAsync<InformativeSended>, IInformativeSendedRepository
    {
        public InformativeSendedRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}