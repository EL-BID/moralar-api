using MongoDB.Bson.Serialization.Attributes;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]

    public class CreditCard : ModelBase
    {
        public string ProfileId { get; set; }
        public string TokenCard { get; set; }

        public override string CollectionName => nameof(CreditCard);

    }
}