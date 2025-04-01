using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Moralar.UtilityFramework.Application.Core.JwtMiddleware
{
    public class TokenProviderOptions
    {
        //
        // Resumen:
        //     The relative request path to listen on.
        //
        // Comentarios:
        //     The default path is /token.
        public string Path { get; set; } = "/token";


        //
        // Resumen:
        //     The Issuer (iss) claim for generated tokens.
        public string Issuer { get; set; }

        //
        // Resumen:
        //     The Audience (aud) claim for the generated tokens.
        public string Audience { get; set; }

        //
        // Resumen:
        //     The expiration time for the generated tokens.
        //
        // Comentarios:
        //     The default is five minutes (300 seconds).
        public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(1.0);


        //
        // Resumen:
        //     The signing key to use when generating tokens.
        public SigningCredentials SigningCredentials { get; set; }

        //
        // Resumen:
        //     Resolves a user identity given a username and pass.
        public Func<string, string, Task<ClaimsIdentity>> IdentityResolver { get; set; }

        //
        // Resumen:
        //     Generates a random value (nonce) for each generated token.
        //
        // Comentarios:
        //     The default nonce is a random GUID.
        public Func<Task<string>> NonceGenerator { get; set; } = () => Task.FromResult(Guid.NewGuid().ToString());

    }

}
