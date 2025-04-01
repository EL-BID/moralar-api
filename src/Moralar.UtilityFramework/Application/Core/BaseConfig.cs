
namespace Moralar.UtilityFramework.Application.Core
{
    public class BaseConfig
    {
        public static string Issuer { get; set; }

        public static string Audience { get; set; }

        public static string SecretKey { get; set; }

        public static bool EnableSwagger { get; set; }

        public static string PasswordKey { get; set; }

        public static string BaseUrl { get; set; }

        public static bool Encrypted { get; set; }

        public static string DefaultUrl { get; set; }

        public static string ApplicationName { get; set; }

        public static string UrlIcon { get; set; }

        public static List<string> CustomUrls { get; set; }

        public static string TokenFrom { get; set; }

        public static double TokenValue { get; set; }

        public static string ProjectFirebaseId { get; set; }

        public BaseConfig()
        {
            CustomUrls = new List<string>();
        }
    }
}
