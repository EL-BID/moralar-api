using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class Question : ModelBase
    {
        public string NameQuestion { get; set; }
        public TypeResponse TypeResponse { get; set; }
        public string QuizId { get; set; }
        public override string CollectionName => nameof(Question);
    }
}
