using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Data.Entities.Auxiliar
{
    public class FamilySpouse
    {
        public string Name { get; set; }
        public long? Birthday { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeGenre? Genre { get; set; } = null;
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeScholarity? SpouseScholarity { get; set; } = null;
    }
}
