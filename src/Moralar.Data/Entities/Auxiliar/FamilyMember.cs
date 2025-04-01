using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Moralar.Data.Enum;

namespace Moralar.Data.Entities.Auxiliar
{
    [BsonIgnoreExtraElements]
    public class FamilyMember
    {
        public string Name { get; set; }
        public long Birthday { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeGenre Genre { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeKingShip KinShip { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeScholarity Scholarity { get; set; }
    }
}
