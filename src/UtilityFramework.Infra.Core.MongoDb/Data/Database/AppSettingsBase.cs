using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using System;
using MongoDB.Driver.Core.Configuration;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Database
{
    public static class AppSettingsBase
    {
        private static IConfigurationRoot Configuration { get; set; }
        private static BaseSettings _settingsDataBase { get; set; }

        public static BaseSettings GetSettings(IHostingEnvironment env)
        {
            var a = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            var baseSettings = new BaseSettings();

            Configuration.GetSection("DATABASE").Bind(baseSettings);

            if (baseSettings.MaxConnections == 0 || baseSettings.MaxConnections == null)
                baseSettings.MaxConnections = 250;

            if (baseSettings.MinConnections == 0 || baseSettings.MinConnections == null)
                baseSettings.MinConnections = 50;

            _settingsDataBase = baseSettings;

            return baseSettings;
        }

        public static BaseSettings GetSettings()
        {
            var a = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            var baseSettings = new BaseSettings();

            Configuration.GetSection("DATABASE").Bind(baseSettings);

            if (baseSettings.MaxConnections == 0 || baseSettings.MaxConnections == null)
                baseSettings.MaxConnections = 250;

            if (baseSettings.MinConnections == 0 || baseSettings.MinConnections == null)
                baseSettings.MinConnections = 50;

            _settingsDataBase = baseSettings;

            return baseSettings;
        }

        public static MongoClient GetMongoClient(BaseSettings settingsDatabase)
        {
            _settingsDataBase = settingsDatabase;


            return new MongoClient(ReadMongoClientSettings());
        }

        private static MongoClientSettings ReadMongoClientSettings()
        {

            MongoClientSettings mongoClientSettings = null;


            mongoClientSettings = MongoClientSettings.FromConnectionString(_settingsDataBase.ConnectionString);

            mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(120);

            //mongoClientSettings.Servers = ListServers();
            mongoClientSettings.UseTls = true;
            //mongoClientSettings.SslSettings = new SslSettings
            //    {
            //        CheckCertificateRevocation = true
            //    };
            mongoClientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
            mongoClientSettings.HeartbeatInterval = TimeSpan.FromSeconds(120);
            mongoClientSettings.HeartbeatTimeout = TimeSpan.FromSeconds(50);

            return mongoClientSettings;
        }
        private static IEnumerable<MongoServerAddress> ListServers()
        {
            var servers = new List<MongoServerAddress>
            {
                BaseSettings.IsDev
                    ? new MongoServerAddress(_settingsDataBase.Remote, 27017)
                    : new MongoServerAddress(_settingsDataBase.Local, 27017)
            };

            return servers;
        }

    }
}