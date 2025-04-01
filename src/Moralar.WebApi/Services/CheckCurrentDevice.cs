using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moralar.Domain;
using Moralar.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Linq;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.WebApi.Services
{
    public class CheckCurrentDevice : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                
                var _familyRepository = (IFamilyRepository)context.HttpContext.RequestServices.GetService(typeof(IFamilyRepository));

                var deviceId = context.HttpContext.Request.Headers["deviceId"].ToString();
                var userId = context.HttpContext.Request.GetUserId();

                var response = new ReturnViewModel();
                response.Erro = true;
                response.Errors = null;
                response.Message = DefaultMessages.Success; 
                var path = context.HttpContext.Request.Path.ToString().ToLower();


                if (string.IsNullOrEmpty(deviceId) == false && string.IsNullOrEmpty(userId) == false)
                {
                    var familyEntity = _familyRepository.FindById(userId);

                    if (familyEntity?.DeviceId != null && familyEntity.DeviceId.Count() > 0 && familyEntity.DeviceId[0] != deviceId)
                    {

                        /*CASO LOGADO EM OUTRO DEVICE DEVE DERRUBAR A SESSÃO*/
                        response.Message = DefaultMessages.InvalidDeviceID;
                        context.Result = new ContentResult()
                        {
                            Content = JsonConvert.SerializeObject(response),
                            StatusCode = 403,
                            ContentType = "application/json"
                        };
                        return;
                    }
                }   return;
            }
            catch (Exception) { } 
        }

    }
}