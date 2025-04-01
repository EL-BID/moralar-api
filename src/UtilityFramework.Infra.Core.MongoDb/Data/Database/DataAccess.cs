using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using UtilityFramework.Infra.Core.MongoDb.Data.Database.Interface;
using UtilityFramework.Infra.Core.MongoDb.Data.Server;

namespace UtilityFramework.Infra.Core.MongoDb.Data.Database
{
    public class DataAccess
    {

        private IMongoDatabase _databaseAsync = null;
        /// <summary>
        /// Return Name connection
        /// </summary>
        public IMongoDatabase DatabaseAccessAsync => _databaseAsync;
        /// <summary>
        /// Return configuration object
        /// </summary>
        private IConfiguration Configuration { get; set; } = null;

        /// <summary>
        /// Return configuration object
        /// </summary>
        private static IHostingEnvironment Env { get; set; } = null;


        /// <summary>
        /// Connection Name
        /// </summary>
        /// <param name="config"></param>
        /// <param name="env"></param>
        public DataAccess(IConfiguration config, IHostingEnvironment env)
        {
            Configuration = config;
            Env = env;

            CreateDatabaseAccess();
        }


        /// <summary>
        /// Create instance to Name
        /// </summary>
        private void CreateDatabaseAccess()
        {
            //_database = LoadMongoServer().GetDatabase(Configuration.DataBaseName);
        }
        private void CreateDatabaseAccessAsync()
        {
            //_databaseAsync = 
        }

        /// <summary>
        /// Return server connection
        /// </summary>
        /// <returns></returns>
        private static MongoServer LoadMongoServer()
        {
            // Return server
            return new ServerAccess(Env).Server;
        }

    }
}
