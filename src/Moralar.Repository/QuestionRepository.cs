using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class QuestionRepository : BusinessBaseAsync<Question>, IQuestionRepository
    {
        public QuestionRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}