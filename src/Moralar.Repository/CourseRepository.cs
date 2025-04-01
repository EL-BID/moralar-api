using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class CourseRepository : BusinessBaseAsync<Course>, ICourseRepository
    {
        public CourseRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}