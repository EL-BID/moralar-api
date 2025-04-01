using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Modelos
{
    [BsonIgnoreExtraElements]
    public abstract class ModelBase
    {
        protected ModelBase()
        {
            Created = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Collection on Name
        /// </summary>
        public abstract string CollectionName { get; }

        /// <summary>
        /// Identification
        /// </summary>
        public ObjectId _id { get; set; }

        /// <summary>
        /// Flag delete
        /// </summary>
        public long? Disabled { get; set; }
        /// <summary>
        /// Data blocked entity
        /// </summary>
        public long? DataBlocked { get; set; }

        public long? Created { get; set; }
        public long? LastUpdate { get; set; }
    }
}