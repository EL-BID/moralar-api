using MongoDB.Driver;
using Newtonsoft.Json;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Database
{
    public class BaseSettings
    {
        [JsonProperty(PropertyName = "CONNECTIONSTRING")]
        public string ConnectionString { get; set; }

        [JsonProperty(PropertyName = "NAME")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "LOCAL")]
        public string Local { get; set; }

        [JsonProperty(PropertyName = "REMOTE")]
        public string Remote { get; set; }

        [JsonProperty(PropertyName = "USER")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "PASSWORD")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "MAX_CONNECTIONS")]
        public int? MaxConnections { get; set; }

        [JsonProperty(PropertyName = "MIN_CONNECTIONS")]
        public int? MinConnections { get; set; }
        [JsonIgnore]
        public static bool IsDev { get; set; }
        public static MongoClient MongoClient { get; set; }
    }
}