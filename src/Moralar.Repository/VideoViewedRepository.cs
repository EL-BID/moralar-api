using Microsoft.AspNetCore.Hosting;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class VideoViewedRepository : BusinessBaseAsync<VideoViewed>, IVideoViewedRepository
    {
        public VideoViewedRepository(IHostingEnvironment env) : base(env)
        {
        }
    }
}