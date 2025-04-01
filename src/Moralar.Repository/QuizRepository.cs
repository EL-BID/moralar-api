using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class QuizRepository : BusinessBaseAsync<Quiz>, IQuizRepository
    {
        public QuizRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}