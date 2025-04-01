using MongoDB.Bson.Serialization.Attributes;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class City : UtilityFramework.Infra.Core.MongoDb.Data.Modelos.City
    {
        public string StateUf { get; set; }
    }
}