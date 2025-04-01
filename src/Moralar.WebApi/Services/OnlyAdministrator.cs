using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.WebApi.Services
{
    public class OnlyAdministrator : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var _userAdministratorRepository = (IUserAdministratorRepository)context.HttpContext.RequestServices.GetService(typeof(IUserAdministratorRepository));
                var userId = context.HttpContext.Request.GetUserId();

                var response = new ReturnViewModel();
                response.Erro = true;
                response.Errors = null;
                response.Message = DefaultMessages.OnlyAdministrator;
                var path = context.HttpContext.Request.Path.ToString().ToLower();


                if (string.IsNullOrEmpty(userId))
                {
                    context.Result = new BadRequestObjectResult(response);
                    return;
                }
                else
                {
                    var userAdministrator = _userAdministratorRepository.FindById(userId);

                    if (userAdministrator == null)
                    {
                        context.Result = new BadRequestObjectResult(response);

                        return;
                    }
                    else if (userAdministrator.DataBlocked != null || userAdministrator.Disabled != null)
                    {
                        response.Message = DefaultMessages.UserAdministratorBlocked;
                        context.Result = new BadRequestObjectResult(response);
                        return;
                    }

                }
            }
            catch (System.Exception) { /*unused*/ }
        }

    }
}