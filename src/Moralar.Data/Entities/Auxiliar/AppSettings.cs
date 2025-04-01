namespace Moralar.Data.Entities.Auxiliar
{
    public class AppSettings
    {
        public JWTSettings Jwt { get; set; }
    }

    public class JWTSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
    }
}
