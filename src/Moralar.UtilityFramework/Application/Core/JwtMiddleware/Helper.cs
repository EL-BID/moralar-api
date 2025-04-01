using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Moralar.UtilityFramework.Application.Core.JwtMiddleware
{
    public static class Helper
    {
        //
        // Resumen:
        //     obtem token
        //
        // Parámetros:
        //   request:
        public static string GetUserId(this HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out var value);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] array = value.ToString().Split(' ');
            value = ((array.Length > 1) ? array[1].Trim() : null);
            string text = (string.IsNullOrEmpty(value) ? null : new JwtSecurityToken(value)?.Subject);
            if (BaseConfig.Encrypted)
            {
                text = Utilities.DecryptString(BaseConfig.SecretKey, text);
            }

            return text;
        }

        public static string GetClaimFromToken(this HttpRequest request, string claimType)
        {
            request.Headers.TryGetValue("Authorization", out var value);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] array = value.ToString().Split(' ');
            value = ((array.Length > 1) ? array[1].Trim() : null);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            try
            {
                return new JwtSecurityToken(value)?.Claims.FirstOrDefault((Claim x) => x.Type.ToLower() == claimType.ToLower())?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<Claim> GetClaimsFromToken(this HttpRequest request)
        {
            List<Claim> result = new List<Claim>();
            try
            {
                request.Headers.TryGetValue("Authorization", out var value);
                if (string.IsNullOrEmpty(value))
                {
                    return result;
                }

                string[] array = value.ToString().Split(' ');
                value = ((array.Length > 1) ? array[1].Trim() : null);
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(value);
                if (jwtSecurityToken == null)
                {
                    return result;
                }

                return jwtSecurityToken.Claims.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
