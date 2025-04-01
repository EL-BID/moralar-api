using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver.Builders;
using Moralar.Data.Entities;
using Moralar.Repository.Interface;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository
{
    public class ResidencialPropertyRepository : BusinessBaseAsync<ResidencialProperty>, IResidencialPropertyRepository
    {
        public ResidencialPropertyRepository(IHostingEnvironment env) : base(env)
        {
            if (GetCollection().IndexExists(IndexKeys.GeoSpatial("Position")) == false)
                GetCollection().CreateIndex(IndexKeys.GeoSpatial("Position"));
        }
    }
}