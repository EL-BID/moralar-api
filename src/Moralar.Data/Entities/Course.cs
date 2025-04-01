using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Enum;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{

    [BsonIgnoreExtraElements]
    public class Course : ModelBase
    {
        public string Title { get; set; }
        public string Img { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public string Schedule { get; set; }
        public string Place { get; set; }
        public string WorkLoad { get; set; }
        public string Description { get; set; }
        public int StartTargetAudienceAge { get; set; }
        public int EndTargetAudienceAge { get; set; }
        public TypeGenre? TypeGenre { get; set; }
        public int NumberOfVacancies { get; set; }

        public override string CollectionName => nameof(Course);
    }
}
