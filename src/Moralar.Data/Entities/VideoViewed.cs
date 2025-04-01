using MongoDB.Bson.Serialization.Attributes;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class VideoViewed : ModelBase
    {
        public string VideoId { get; set; }
        public string FamilyId { get; set; }
        public override string CollectionName => nameof(VideoViewed);
    }
}