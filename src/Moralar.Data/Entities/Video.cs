using MongoDB.Bson.Serialization.Attributes;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class Video : ModelBase
    {
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public override string CollectionName => nameof(Video);
    }
}