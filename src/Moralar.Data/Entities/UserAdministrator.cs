using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Enum;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]

    public class UserAdministrator : ModelBase
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public TypeUserProfile TypeProfile { get; set; }

        public override string CollectionName => nameof(UserAdministrator);
    }
}