using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Enum;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class Notification : ModelBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public long? DateViewed { get; set; }
        public string FamilyId { get; set; }
        public ForType? For { get; set; }
        public string? Module { get; set; }

        public override string CollectionName => nameof(Notification);

        public string? SeenBy { get; set; }
    }

}
