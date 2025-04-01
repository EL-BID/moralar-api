using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class QuestionAnswerRepository : BusinessBaseAsync<QuestionAnswer>, IQuestionAnswerRepository
    {
        public QuestionAnswerRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}