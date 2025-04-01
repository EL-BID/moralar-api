using System.Collections.Generic;
using MongoDB.Driver;
using UtilityFramework.Infra.Core.MongoDb.Data.Server.Interface;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Server
{
    public class ConfigurationServer : IConfigurationServer
    {
        public ConfigurationServer()
        {
            Servers = new List<MongoServerAddress>();
        }

        /// <summary>
        /// Main server
        /// </summary>
        public MongoServerAddress Server { get; set; }

        /// <summary>
        /// Failover servers
        /// </summary>
        public List<MongoServerAddress> Servers {get; set;}
    }
}