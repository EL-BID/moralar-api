using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class QuizFamilyRepository : BusinessBaseAsync<QuizFamily>, IQuizFamilyRepository
    {
        public QuizFamilyRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}