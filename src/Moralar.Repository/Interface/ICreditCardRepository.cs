using Moralar.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository.Interface
{
    public interface ICreditCardRepository : IBusinessBaseAsync<CreditCard>
    {
        Task<IEnumerable<CreditCard>> ListCreditCard(string profileId);
    }
}