using System.Collections.Generic;
using MongoDB.Driver;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Server.Interface
{
    public interface IConfigurationServer
    {
        /// <summary>
        /// Main server or proxy
        /// </summary>
        MongoServerAddress Server { get; set; }

        /// <summary>
        /// Alternative destinations
        /// </summary>
        List<MongoServerAddress> Servers { get; set; }
    }
}