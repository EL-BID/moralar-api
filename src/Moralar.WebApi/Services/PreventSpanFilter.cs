using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.WebApi.Services
{
    public class PreventSpanFilter : ActionFilterAttribute
    {
        public int DelayRequest { get; set; } = 1;
        public string ErrorMessage { get; set; } = null;
        private List<string> ignoreEndPoints = new List<string>() { "trigger/changestatusgatway", "loaddata","searchtimeline","registerunregisterdeviceid" };



        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {

                if (filterContext.HttpContext.Request.Method.ToLower() == "get" || (filterContext.HttpContext.Request.HasFormContentType && filterContext.HttpContext.Request.Form.ContainsKey("columns")) || (ignoreEndPoints.Count(endpoint => filterContext.HttpContext.Request.Path.Value.IndexOf(endpoint, StringComparison.OrdinalIgnoreCase) != -1) > 0))
                    return;

                if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    if (controllerActionDescriptor.MethodInfo.CustomAttributes.Count(x => x.AttributeType == typeof(PreventSpanFilter)) > 0 && (DelayRequest == 1 && string.IsNullOrEmpty(ErrorMessage)))
                        return;
                }

                var cacheService = filterContext.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

                //Store our HttpContext (for easier reference and code brevity)
                var request = filterContext.HttpContext.Request;

                //Grab the IP Address from the originating Request (very simple implementation for example purposes)
                var originationInfo = request.HttpContext.Connection.RemoteIpAddress.ToString();

                //Append the User Agent
                if (request.Headers.Count(x => x.Key == "User-Agent") > 0)
                    originationInfo += request.Headers["User-Agent"].ToString();

                try
                {
                    //Append the Authorization
                    if (request.Headers.Count(x => x.Key == "Authorization") > 0)
                        originationInfo += request.Headers["Authorization"].ToString();
                }
                catch (Exception)
                { }

                //Now we just need the target URL Information
                var targetInfo = UriHelper.GetDisplayUrl(request);

                //Generate a hash for your strings (this appends each of the bytes of the value into a single hashed string
                var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

                //Checks if the hashed value is contained in the Cache (indicating a repeat request)
                if (cacheService.TryGetValue(hashValue, out var resultCache))
                {
                    var response = new ReturnViewModel()
                    {
                        Erro = true,
                        Message = ErrorMessage ?? $"Você realizou essa mesma requisição em menos de {DelayRequest}s"
                    };

                    filterContext.Result = new BadRequestObjectResult(response);
                }
                else
                {
                    //Adds an empty object to the cache using the hashValue to a key (This sets the expiration that will determine
                    //if the Request is valid or not
                    var cache = cacheService.GetOrCreate(hashValue, entry =>
                    {
                        entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(DelayRequest));
                        entry.Priority = CacheItemPriority.Normal;
                        return entry;
                    });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}