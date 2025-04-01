using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Services.Core.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moralar.UtilityFramework.Application.Core;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;

// ReSharper disable RedundantAnonymousTypePropertyName

namespace Moralar.Domain.Services
{

    public class UtilService : ControllerBase, IUtilService
    {
        private readonly IMapper _mapper;
        private readonly ILogActionRepository _logActionRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ISenderMailService _senderMailService;
        private readonly ISenderNotificationService _senderNotificationService;

        private readonly IServiceProvider _serviceProvider;

        private readonly IProfileRepository _profileRepository;


        public UtilService(IMapper mapper, ILogActionRepository logActionRepository, ICityRepository cityRepository, ISenderMailService senderMailService, ISenderNotificationService senderNotificationService, INotificationRepository notificationRepository, IServiceProvider serviceProvider, IProfileRepository profileRepository)
        {
            var mapper2 = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IMapper>();
            _mapper = mapper2;
            _logActionRepository = logActionRepository;
            _cityRepository = cityRepository;
            _senderMailService = senderMailService;
            _senderNotificationService = senderNotificationService;
            _notificationRepository = notificationRepository;

            _serviceProvider = serviceProvider;

            _profileRepository = profileRepository;
        }
        public string GetEscolasMunicipaisEnsinoFundamental()
        {

            return "";
        }
        public string GetFlag(string flag)
        {
            switch (flag)
            {
                case "amex":
                    return $"{BaseConfig.CustomUrls[0]}content/images/flagcard/{flag.ToLower()}.png";
                case "dinners":
                    return $"{BaseConfig.CustomUrls[0]}content/images/flagcard/{flag.ToLower()}.png";
                case "mastercard":
                case "master":
                    return $"{BaseConfig.CustomUrls[0]}content/images/flagcard/mastercard.png";
                case "discover":
                    return $"{BaseConfig.CustomUrls[0]}content/images/flagcard/{flag.ToLower()}.png";
                default:
                    return $"{BaseConfig.CustomUrls[0]}content/images/flagcard/visa.png";
            }
        }

        public static JToken GetJsonData(string fileName, bool defaultt = false)
        {
            JToken data;
            // another alternative of getEncoding value its: iso-8859-1
            using (StreamReader sr = new StreamReader($"Content/jsons/{fileName}", defaultt ? Encoding.Default : Encoding.GetEncoding("iso-8859-1"), true))
            {
                string json = sr.ReadToEnd();
                data = JToken.Parse(json);
            }
            return data;
        }

        public async Task LogUserAdministrationAction(string userId, string message, TypeAction typeAction, LocalAction localAction, string referenceId = null)
        {
            try
            {

                //var userAdministratorEntity = await _userAdministratorRepository.FindByIdAsync(userId);

                //var entityLogAction = new LogAction()
                //{
                //    TypeAction = typeAction,
                //    TypeResposible = TypeResposible.UserAdminstrator,
                //    ReferenceId = referenceId,
                //    Message = message,
                //    ResponsibleId = userId,
                //    ResponsibleName = userAdministratorEntity.Name,
                //    Justification = null,
                //    LocalAction = localAction,
                //    ClientIp = Util.GetClientIp()
                //};

                //await _logActionRepository.CreateReturnAsync(entityLogAction);



            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RegisterLogAction(LocalAction localAction, TypeAction typeAction, TypeResposible typeResposible, string message, string responsibleId = null, string responsibleName = null, string referenceId = null, string justification = null, Exception ex = null)
        {
            try
            {
                var entityLogAction = new LogAction()
                {
                    TypeAction = typeAction,
                    TypeResposible = typeResposible,
                    ReferenceId = referenceId,
                    Message = message,
                    ResponsibleId = responsibleId,
                    ResponsibleName = responsibleName,
                    Justification = justification,
                    StackTrace = ex?.StackTrace,
                    MessageEx = ex != null ? $"{ex.InnerException} {ex.Message}".Trim() : null,
                    LocalAction = localAction,
                    ClientIp = "127.0.0.1"
                };

                await _logActionRepository.CreateReturnAsync(entityLogAction);

            }
            catch (Exception)
            {
                //unused
            }
        }

        public void UpdateCascate(Family familyEntity)
        {
            try
            {
                //var id = entity._id.ToString();

                //_userAdministratorRepository.UpdateMultiple(Query<Family>.EQ(x => x.AgentId, id),
                //new UpdateBuilder<UserAdministrator>().Set(x => x.AgentName, entity.ContactName),
                //UpdateFlags.Multi);
            }
            catch (Exception)
            {
                /*unused*/
            }
        }

        public static string? getModuleSchedule(TypeScheduleStatus status, string scheduleId)
        {
            if (scheduleId != "") {
                return $"app.agendamento.{(int) (status)}/{scheduleId}";
            } else {
                return $"app.agendamento.{status}";
            }
        }

        public static string? getModuleMatch(string propertyInterestId)
        {
            if (propertyInterestId != "")
            {
                return $"app.match.0/{propertyInterestId}";
            }
            else
            {
                return $"app.match.0";
            }
        }

        public static string? getModuleMatchContemplate(string id, string residencialCode)
        {
            if (id != "" && residencialCode != null)
            {
                return $"app.match.0/{id}?id2={residencialCode}";
            }
            else
            {
                return $"app.match.0/{id}";
            }
        }

        public static string? getModuleMatchSelled(string id, string id2)
        {
            if (id != "" && id2 != null)
            {
                return $"app.match-selled.0/{id}/{id2}";
            }
            else
            {
                return $"app.match-selled.0/{id}";
            }
        }

        public static string? getModuleNotification()
        {
            return $"app.notification.0/0";
        }

        public static string? getModuleQuestSended(string id, string id2)
        {
            if (id != "" && id2 != null)
            {
                return $"app.quest-sended.0/{id}/{id2}";
            }
            else
            {
                return $"app.quest-sended.0/{id}";
            }
        }


        public async Task<InfoAddressViewModel> GetInfoFromZipCode(string zipCode)
        {
            try
            {
                return await getAddressFromZipCode(zipCode);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task SendNotify(string title, string content, string email,
            List<string> deviceId, ForType fortype = ForType.Family, 
            string familyId = null, string titlePush = null, string contentPush = null, string? module = null, string? seenBy = null)
        {
            try
            {
                /*GERAR NOTIFICAÇÃO*/

                var notificationFamilyEntity = new Notification()
                {
                    FamilyId = fortype == ForType.Family ? familyId : null,
                    For = fortype,
                    Title = titlePush ?? title,
                    Description = contentPush ?? content,
                    Module = module,
                    SeenBy = seenBy,
                };

                await _notificationRepository.CreateAsync(notificationFamilyEntity);

                if (deviceId != null && deviceId.Count() > 0)
                {
                    dynamic payLoadPush = Util.GetPayloadPush();
                    dynamic settingsPush = Util.GetSettingsPush();

                    await _senderNotificationService.SendPushAsync(title, content, deviceId, data: payLoadPush, settings: settingsPush, priority: 10);
                }

            }
            catch (Exception) {/*UNUSED*/}

            try
            {
                if (string.IsNullOrEmpty(email) == false)
                {
                    /*ENVIO DE EMAIL*/
                    var dataBody = Util.GetTemplateVariables();

                    dataBody.Add("{{ title }}", title);
                    dataBody.Add("{{ message }}", content);

                    var body = _senderMailService.GerateBody("custom", dataBody);

                    await _senderMailService.SendMessageEmailAsync("Moralar", email, body, title);
                }

            }
            catch (Exception) {/*UNUSED*/}


        }

        public static string getFilePathFromServer(string folderPath)
        {
            bool isAzureServer = true;

            string rootPath;
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME")))
                rootPath = Environment.GetEnvironmentVariable("HOME").ToString() + "site/wwwroot";
            else
                rootPath = Directory.GetCurrentDirectory();
            return Path.Combine(rootPath, folderPath);
        }

        public async Task sentNotificationOfContemplateToEquipeTTSAsync(Family family, string id, string residencialCode)
        {
            bool sendNotify = true;
            ForType forType = ForType.TTS;
            string title = "Um imóvel foi contemplado";

            var message = new StringBuilder();
            string content = null;

            message.AppendLine("<p>Olá equipe TTS,<br/></p>");
            message.AppendLine($"Eles consideraram a família {family.Holder.Number} para el imóvel #{residencialCode}:");
            content = message.ToString();

            if (sendNotify)
            {
                await SendNotify(title, content, null, family.DeviceId, forType, family._id.ToString(), null, null,
                    UtilService.getModuleMatchContemplate(id, residencialCode));
            }

            var entityProfile2 = await _profileRepository.FindByAsync(x => x.TypeProfile == TypeUserProfile.Gestor || x.TypeProfile == TypeUserProfile.TTS);
            foreach (var item in entityProfile2)
            {
                string content2 = null;
                var message2 = new StringBuilder();

                message2.AppendLine($"<p>Olá {item.Name},<br/></p>");
                message2.AppendLine($"Eles consideraram a família {family.Holder.Number} para el imóvel #{residencialCode}:");
                content2 = message2.ToString();

                var dataBody2 = Util.GetTemplateVariables();
                dataBody2.Add("{{ title }}", title);
                dataBody2.Add("{{ message }}", content2);

                var body2 = _senderMailService.GerateBody("custom", dataBody2);

                var unused = Task.Run(async () =>
                {
                    await _senderMailService.SendMessageEmailAsync("Moralar", item.Email, body2, title).ConfigureAwait(false);
                });

            }
        }

        public async Task<InfoAddressViewModel?> getAddressFromZipCode(string zipCode)
        {
            var client = new RestClient($"http://viacep.com.br/ws/{zipCode.OnlyNumbers()}/json/");

            var request = new RestRequest(Method.GET);

            var infoZipCode = await client.Execute<AddressInfoViewModel>(request).ConfigureAwait(false);

            if (infoZipCode.Data == null || infoZipCode.Data.Erro == "True")
                return null;

            var response = _mapper.Map<InfoAddressViewModel>(infoZipCode.Data);

            var cities = UtilService.GetJsonData("cities-br.json");


            var city = cities
                .Where(x => x["name"].ToString().Equals(infoZipCode.Data.Localidade))
                .ToList().FirstOrDefault();

            var states = UtilService.GetJsonData("list-state-br.json");


            var state = states
                .Where(x => x["name"].ToString().Equals(city.Value<string>("stateName")))
                .ToList().FirstOrDefault();

            if (city == null || state == null)
                return response;

            response.CityId = city.Value<string>("id");
            response.CityName = city.Value<string>("name");
            response.StateId = state.Value<string>("id");
            response.StateUf = infoZipCode.Data.Uf;
            response.StateName = state.Value<string>("name");

            return response;
        }

    }
}