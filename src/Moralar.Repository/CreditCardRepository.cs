using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class CreditCardRepository : BusinessBaseAsync<CreditCard>, ICreditCardRepository
    {
        public async Task<IEnumerable<CreditCard>> ListCreditCard(string profileId) => await FindByAsync(x => x.ProfileId == profileId, Builders<CreditCard>.Sort.Descending(x => x.Created));

        public CreditCardRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}