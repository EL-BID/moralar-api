using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Server
{

    public class MongoDbContext : IMongoDbContext
    {
        private IMongoDatabase _db;
        private MongoClient _client;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _db = _client.GetDatabase(settings.Value.Name);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }

    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }

    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
    }
}
