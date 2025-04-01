using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Entities.Auxiliar;
using System;
using System.Collections.Generic;
using System.Text;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class PropertiesInterest : ModelBase
    {
        public string FamilyId { get; set; }
        public string ResidencialPropertyId { get; set; }
        public string ResidencialCode { get; set; }
        public string HolderName { get; set; }
        public string HolderEmail { get; set; }
        public string HolderCpf { get; set; }
        public string HolderNumber { get; set; }
        public FamilyPriorization Priorization { get; set; }
        public override string CollectionName => nameof(PropertiesInterest);
    }
}
