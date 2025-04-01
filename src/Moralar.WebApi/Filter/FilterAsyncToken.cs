using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Moralar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;

namespace Moralar.WebApi.Filter
{
    public class FilterAsyncToken : ControllerBase, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                var userId = context.HttpContext.Request.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    context.Result = BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));
            }
            if (context.Result == null)
                await next();
        }
    }
}
