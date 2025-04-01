using MongoDB.Bson.Serialization.Attributes;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class State : UtilityFramework.Infra.Core.MongoDb.Data.Modelos.State
    {
        public string CountryId { get; set; }
        public string CountryName { get; set; }
    }
}