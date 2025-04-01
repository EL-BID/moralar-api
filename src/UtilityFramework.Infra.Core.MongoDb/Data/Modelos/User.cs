using MongoDB.Bson.Serialization.Attributes;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Modelos
{
    [BsonIgnoreExtraElements]
    public class User : ModelBase
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string FacebookId { get; set; }
        public override string CollectionName => nameof(User);
    }
}