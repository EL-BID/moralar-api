
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Principal;
using Microsoft.VisualBasic;
using Moralar.UtilityFramework.Configuration;

namespace Moralar.UtilityFramework.Application.Core.JwtMiddleware
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;

        private static TokenProviderOptions _options;


        Func<string, string, Task<ClaimsIdentity>> identityResolver = async (username, password) => {
            return new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { });
        };

        public TokenProviderMiddleware(IOptions<TokenProviderOptions> options, RequestDelegate next)
        {
            // TODO: not need it but its a part of a .net1.1 version
            //var configHelper = new ConfigurationHelper();
            //var settings = configHelper.GetSettings();

            _next = next;
            _options = options.Value;

            _options.Issuer = "Moralar";
            _options.Audience = "Moralar";
            var signingKey = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes("ebbabd773c23a7e55415c64cb40b2f92")), SecurityAlgorithms.HmacSha256);

            var signingCredentials = signingKey;
            Func<Task<string>> nonceGenerator = () => Task.FromResult(Guid.NewGuid().ToString());
            _options.IdentityResolver = identityResolver;
            _options.SigningCredentials = signingCredentials;
            _options.NonceGenerator = nonceGenerator;

            ThrowIfInvalidOptions(_options);
        }

        //
        // Resumen:
        //     MIDDLEWARE REQUEST
        //
        // Parámetros:
        //   context:
        public Task Invoke(HttpContext context)
        {
            return _next(context);
        }

        //
        // Resumen:
        //     GERATE TOKEN AND REFRESH TOKEN PROFILE
        //
        // Parámetros:
        //   profileId:
        //     IDENTIFIER PROFILE
        //
        //   refreshToken:
        //     DEFAULT FALSE
        //
        //   customClaim:
        public static object GenerateToken(string profileId, bool refreshToken = false, params Claim[] customClaim)
        {
            try
            {
                if (BaseConfig.Encrypted)
                {
                    profileId = Utilities.EncryptString(BaseConfig.SecretKey, profileId);
                }

                DateTime utcNow = DateTime.UtcNow;
                List<Claim> list = new List<Claim>
            {
                new Claim("sub", profileId),
                new Claim("jti", default(Guid).ToString()),
                new Claim("iat", ToUnixEpochDate(utcNow).ToString(), "http://www.w3.org/2001/XMLSchema#integer64")
            };
                if (customClaim != null && customClaim.Length != 0)
                {
                    list.AddRange(customClaim);
                }

                DateTime value = ((!refreshToken) ? utcNow.Add(_options.Expiration) : utcNow.Add(_options.Expiration).Add(TimeSpan.FromHours(2.0)));
                JwtSecurityToken token = new JwtSecurityToken(_options.Issuer, _options.Audience, list, utcNow, value, _options.SigningCredentials);
                JwtSecurityToken token2 = new JwtSecurityToken(_options.Issuer, _options.Audience, list, utcNow, utcNow.Add(_options.Expiration.Add(TimeSpan.FromHours(2.0))), _options.SigningCredentials);
                string access_token = new JwtSecurityTokenHandler().WriteToken(token);
                string refresh_token = new JwtSecurityTokenHandler().WriteToken(token2);
                return new
                {
                    access_token = access_token,
                    refresh_token = refresh_token,
                    expires_in = (int)_options.Expiration.TotalSeconds,
                    expires = $"{DateTime.Now.AddSeconds(_options.Expiration.TotalSeconds):dd/MM/yyyy HH:mm:ss}",
                    expires_type = "seconds"
                };
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Path");
            }
        }

        //
        // Resumen:
        //     GERATE CUSTOM TOKEN
        //
        // Parámetros:
        //   profileId:
        //     IDENTIFIER PROFILE
        //
        //   path:
        //     PATH API OFF TOKEN
        //
        //   expiration:
        //
        //   refreshToken:
        //     DEFAULT FALSE
        //
        //   customClaim:
        public static object GenerateTokenCustom(string profileId, TimeSpan? expiration = null, bool refreshToken = false, params Claim[] customClaim)
        {
            try
            {
                if (BaseConfig.Encrypted)
                {
                    profileId = Utilities.EncryptString(BaseConfig.SecretKey, profileId);
                }

                DateTime utcNow = DateTime.UtcNow;
                if (!expiration.HasValue)
                {
                    expiration = _options.Expiration;
                }

                List<Claim> list = new List<Claim>
            {
                new Claim("sub", profileId),
                new Claim("jti", default(Guid).ToString()),
                new Claim("iat", ToUnixEpochDate(utcNow).ToString(), "http://www.w3.org/2001/XMLSchema#integer64")
            };
                if (customClaim != null && customClaim.Length != 0)
                {
                    list.AddRange(customClaim);
                }

                DateTime value = ((!refreshToken) ? utcNow.Add(expiration.GetValueOrDefault()) : utcNow.Add(expiration.GetValueOrDefault()).Add(TimeSpan.FromHours(2.0)));
                JwtSecurityToken token = new JwtSecurityToken(_options.Issuer, _options.Audience, list, utcNow, value, _options.SigningCredentials);
                JwtSecurityToken token2 = new JwtSecurityToken(_options.Issuer, _options.Audience, list, utcNow, utcNow.Add(expiration.GetValueOrDefault().Add(TimeSpan.FromHours(2.0))), _options.SigningCredentials);
                string access_token = new JwtSecurityTokenHandler().WriteToken(token);
                string refresh_token = new JwtSecurityTokenHandler().WriteToken(token2);
                return new
                {
                    access_token = access_token,
                    refresh_token = refresh_token,
                    expires_in = (int)expiration.GetValueOrDefault().TotalSeconds,
                    expires = $"{DateTime.Now.AddSeconds(expiration.GetValueOrDefault().TotalSeconds):dd/MM/yyyy HH:mm:ss}",
                    expires_type = "seconds"
                };
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Path");
            }
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException("Path");
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException("Issuer");
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException("Audience");
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", "Expiration");
            }

            if (options.IdentityResolver == null)
            {
                throw new ArgumentNullException("IdentityResolver");
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException("SigningCredentials");
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException("NonceGenerator");
            }
        }

        //
        // Resumen:
        //     Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight
        //     UTC).
        //
        // Parámetros:
        //   date:
        //     The date to convert.
        //
        // Devuelve:
        //     Seconds since Unix epoch.
        private static long ToUnixEpochDate(DateTime date)
        {
            return new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
        }

        //
        // Resumen:
        //     METODO PARA VALIDAR TOKEN
        //
        // Parámetros:
        //   authToken:
        //
        //   validExpiration:
        public static bool ValidateToken(string authToken, bool validExpiration = true)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = GetValidationParameters(validExpiration);
            jwtSecurityTokenHandler.ValidateToken(authToken, validationParameters, out var _);
            return true;
        }

        //
        // Resumen:
        //     METODO PARA RENOVAR TOKEN
        //
        // Parámetros:
        //   authToken:
        //
        //   refreshToken:
        //
        //   customClaim:
        public static IActionResult RefreshToken(string authToken, bool refreshToken = false, params Claim[] customClaim)
        {
            try
            {
                authToken = ((!string.IsNullOrEmpty(authToken)) ? Regex.Replace(authToken, "bearer", "", RegexOptions.IgnoreCase).Trim() : null);
                if (string.IsNullOrEmpty(authToken))
                {
                    return new BadRequestObjectResult(Utilities.ReturnErro("Credênciais inválidas."));
                }

                ValidateToken(authToken);
                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(authToken);
                if (!string.IsNullOrEmpty(jwtSecurityToken?.Subject))
                {
                    return new OkObjectResult(Utilities.ReturnSuccess("Sucesso", GenerateToken(jwtSecurityToken?.Subject, refreshToken, customClaim)));
                }

                return new BadRequestObjectResult(Utilities.ReturnErro("Credênciais inválidas."));
            }
            catch (SecurityTokenExpiredException ex)
            {
                return new BadRequestObjectResult(ex.ReturnErro("Sessão expirada"));
            }
            catch (SecurityTokenInvalidSignatureException ex2)
            {
                return new BadRequestObjectResult(ex2.ReturnErro("Token com assinatura Inválida"));
            }
            catch (Exception ex3)
            {
                return new BadRequestObjectResult(ex3.ReturnErro("Token Inválido"));
            }
        }

        //
        // Resumen:
        //     METODO PARA OBTER METODO DE VALIDAÇÃO DO TOKEN
        //
        // Parámetros:
        //   validExpiration:
        private static TokenValidationParameters GetValidationParameters(bool validExpiration = true)
        {
            SymmetricSecurityKey issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(BaseConfig.SecretKey));
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                ValidateIssuer = true,
                ValidIssuer = BaseConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = BaseConfig.Audience,
                ValidateLifetime = validExpiration,
                ClockSkew = TimeSpan.Zero
            };
        }
    }

}
