using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Data.Entities.Auxiliar
{
    public class FamilyHolder
    {
        public string _id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public long? Birthday { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeGenre? Genre { get; set; } = null;
        public string Email { get; set; }
        public string Phone { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public TypeScholarity? Scholarity { get; set; } = null;
        
    }
}
