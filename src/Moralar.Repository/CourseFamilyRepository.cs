using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class CourseFamilyRepository : BusinessBaseAsync<CourseFamily>, ICourseFamilyRepository
    {
        public CourseFamilyRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}