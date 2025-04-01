using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using UtilityFramework.Infra.Core.MongoDb.Data.Database;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Server
{
    public class ServerAccess
    {
        /// <summary>
        /// Local connection
        /// </summary>
        private static MongoServer _server = null;
        private object _objLock = false;
        private static BaseSettings _baseSettings;
        /// <summary>
        /// Return current server
        /// </summary>
        /// <returns></returns>
        public MongoServer Server => _server;

        /// <summary>
        /// Create server connection
        /// </summary>
        public ServerAccess(IHostingEnvironment env)
        {
            _baseSettings = AppSettingsBase.GetSettings(env);
            CreateServerConnection();
        }

        /// <summary>
        /// Create connection with server
        /// </summary>
        private void CreateServerConnection()
        {
            MongoClient mongoclient = new MongoClient(ReadMongoClientSettings());

            _server = mongoclient.GetServer();

        }
        private void CreateServerConnectionAsync()
        {

        }

        private MongoClientSettings ReadMongoClientSettings()
        {

            var _return = new MongoClientSettings
            {
                ConnectionMode = ConnectionMode.Automatic,
                Servers = ListServers(),
                MaxConnectionPoolSize = 250,
                MinConnectionPoolSize = 50
            };

            if (string.IsNullOrEmpty(_baseSettings.Password) || string.IsNullOrEmpty(_baseSettings.User))
                return _return;

            var mongoCredential = MongoCredential.CreatePlainCredential(_baseSettings.Name,
                _baseSettings.User, _baseSettings.Password);

            _return.Credentials = new List<MongoCredential>() { mongoCredential };

            return _return;
        }

        /// <summary>
        /// Return server list avaiable
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<MongoServerAddress> ListServers()
        {
            var config = new ConfigurationServer();

            if (!string.IsNullOrEmpty(_baseSettings.User) && !string.IsNullOrEmpty(_baseSettings.Password))
            {
                config.Servers.Add(BaseSettings.IsDev
                    ? new MongoServerAddress(_baseSettings.Remote, 27017)
                    : new MongoServerAddress(_baseSettings.Local, 27017));
            }


            config.Servers.Add(BaseSettings.IsDev
                ? new MongoServerAddress(_baseSettings.Remote, 27017)
                : new MongoServerAddress(_baseSettings.Local, 27017));
            return config.Servers;
        }
    }
}