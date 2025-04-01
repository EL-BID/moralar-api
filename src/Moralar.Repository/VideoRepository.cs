using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class VideoRepository : BusinessBaseAsync<Video>, IVideoRepository
    {
        public VideoRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}