using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Enum;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class NotificationSended : ModelBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<string> FamilyId { get; set; } = new List<string>();
        public bool AllFamily { get; set; }
        public override string CollectionName => nameof(NotificationSended);
    }
}
