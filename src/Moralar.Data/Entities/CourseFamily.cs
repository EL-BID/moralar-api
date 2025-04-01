using MongoDB.Bson.Serialization.Attributes;

using Moralar.Data.Enum;

using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{

    [BsonIgnoreExtraElements]
    public class CourseFamily : ModelBase
    {
        public string FamilyId { get; set; }
        public string HolderName { get; set; }
        public string CourseId { get; set; }
        public TypeStatusCourse TypeStatusCourse { get; set; }
        public override string CollectionName => nameof(CourseFamily);
    }
}
