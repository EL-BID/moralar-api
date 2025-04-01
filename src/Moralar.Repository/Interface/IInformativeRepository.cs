using Moralar.Data.Entities;
using System.Threading.Tasks;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository.Interface
{
    public interface IInformativeRepository : IBusinessBaseAsync<Informative>
    {        
    }
}