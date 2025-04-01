using Microsoft.Extensions.Configuration;
using Moralar.Data.Entities.Auxiliar;

namespace Moralar.UtilityFramework.Configuration
{
    public class ConfigurationHelper
    {
        private readonly IConfiguration _configuration;

        public ConfigurationHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public AppSettings GetSettings()
        {
            var settings = new AppSettings();

            settings.Jwt.Issuer = _configuration.GetSection("Jwt:Issuer").Value;
            settings.Jwt.Audience = _configuration.GetSection("Jwt:Audience").Value;
            settings.Jwt.SecretKey = _configuration.GetSection("Jwt:SecretKey").Value;

            return settings;
        }

        public string getBaseUrl()
        {
            var a = _configuration.GetSection("Config:BaseUrl").Value;
            return a;
        }
    }
}
