using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.WebApi.Services
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var allowAccess = Utilities.GetConfigurationRoot().GetSection("allowAccess")?.Get<List<string>>() ?? new List<string>();


            return allowAccess.Count == 0 || allowAccess.Count(x => x == context.Request.RemoteIpAddress) > 0;
        }
    }
}