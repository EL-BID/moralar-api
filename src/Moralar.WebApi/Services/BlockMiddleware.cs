using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;

namespace Moralar.WebApi.Services
{
    public class BlockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserAdministratorRepository _userAdministratorRepository;
        public BlockMiddleware(RequestDelegate next, IProfileRepository profileRepository, IUserAdministratorRepository userAdministratorRepository)
        {
            _next = next;
            _profileRepository = profileRepository;
            _userAdministratorRepository = userAdministratorRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var userId = context.Request.GetUserId();
                var isBlocked = false;
                var isNotFound = false;
                string customMessage = null;

                if (context.Request.Body != null)
                {
                    context.Request.EnableBuffering();
                    var reqBody = context.Request.Body;
                    var reader = new StreamReader(reqBody, Encoding.UTF8);
                    var reqContent = await reader.ReadToEndAsync();
                    reqBody.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = reqBody;
                }

                if (context.Request.Path.Value.ToLower().Contains("api/") && string.IsNullOrEmpty(userId) == false)
                {

                    var role = context.Request.GetRole().ToString();

                    switch (role)
                    {
                        case nameof(TypeProfile.Gestor):
                            var userAdministratorEntity = await _userAdministratorRepository.FindByIdAsync(userId);

                            if (userAdministratorEntity == null)
                            {
                                customMessage = DefaultMessages.UserAdministratorNotFound;
                                isNotFound = true;
                            }
                            else if (userAdministratorEntity.DataBlocked != null)
                            {
                                customMessage = DefaultMessages.AccessBlocked;
                                isBlocked = true;
                            }
                            break;
                        default:
                            var profileEntity = await _profileRepository.FindByIdAsync(userId);

                            if (profileEntity == null)
                            {
                                customMessage = DefaultMessages.ProfileNotFound;
                                isNotFound = true;
                            }
                            else if (profileEntity.DataBlocked != null)
                            {
                                customMessage = DefaultMessages.AccessBlocked;
                                isBlocked = true;
                            }
                            break;
                    }

                }
                if (isNotFound)
                {
                    await MapErro(context, 400, null, customMessage);
                    return;

                }
                else if (isBlocked)
                {
                    await MapErro(context, 423, null, customMessage);
                    return;
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                await MapErro(context, (int)HttpStatusCode.InternalServerError, ex);
            }
        }

        private static async Task MapErro(HttpContext context, int statusCode, Exception ex = null, string customMessage = null)
        {
            using (var newBody = new MemoryStream())
            {
                var response = ex != null ? ex.ReturnErro(DefaultMessages.MessageException) : Utilities.ReturnErro(customMessage ?? DefaultMessages.AccessBlocked);

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
        }
    }

    public static class BlockMiddlewareExtensions
    {
        public static IApplicationBuilder UseBlockMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<BlockMiddleware>();
    }
}