using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class QuestionDescriptionRepository : BusinessBaseAsync<QuestionDescription>, IQuestionDescriptionRepository
    {
        public QuestionDescriptionRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}