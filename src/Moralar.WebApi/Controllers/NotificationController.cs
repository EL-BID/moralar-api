using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Family;
using Moralar.Domain.ViewModels.Notification;
using Moralar.Domain.ViewModels.NotificationSended;
using Moralar.Repository;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;

namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class NotificationController : Controller
    {

        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationSendedRepository _notificationSendedRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly ISenderMailService _senderMailService;
        private readonly IUtilService _utilService;

        public NotificationController(IMapper mapper, INotificationRepository notificationRepository, INotificationSendedRepository notificationSendedRepository, IFamilyRepository familyRepository, ISenderMailService senderMailService, IUtilService utilService)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _notificationSendedRepository = notificationSendedRepository;
            _familyRepository = familyRepository;
            _senderMailService = senderMailService;
            _utilService = utilService;
        }

        /// <summary>
        /// Marca como leído la notificación
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("SetHasBeenRead/{id}/{userId}")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetHasBeenRead([FromRoute] string id, [FromRoute] string userId)
        {
            try
            {
                var notiToUpdate = _notificationRepository.FindById(id);

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();


                if (string.IsNullOrEmpty(notiToUpdate.SeenBy))
                {
                    notiToUpdate.SeenBy = userId;
                }
                else if (!notiToUpdate.SeenBy.Split(',').Contains(userId))
                {
                    notiToUpdate.SeenBy = notiToUpdate.SeenBy + $",{userId}";
                }

                notiToUpdate.DateViewed = now;

                _notificationRepository.Update(notiToUpdate);

                return Ok(Utilities.ReturnSuccess(nameof(DefaultMessages.Updated)));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }


        ///// <summary>
        ///// METODO DE DETALHES DO ITEM
        ///// </summary>
        ///// <response code="200">Returns success</response>
        ///// <response code="400">Custom Error</response>
        ///// <response code="401">Unauthorize Error</response>
        ///// <response code="500">Exception Error</response>
        ///// <returns></returns>
        [HttpGet("DetailNotification/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DetailNotification([FromRoute] string id)
        {
            try
            {
                var Entity = await _notificationRepository.FindByIdAsync(id).ConfigureAwait(false);
                if (Entity == null)
                    return BadRequest(Utilities.ReturnErro(nameof(DefaultMessages.InformativeNotFound)));

                var responseViewModel = _mapper.Map<NotificationListViewModel>(Entity);

                return Ok(Utilities.ReturnSuccess(data: responseViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }
        ///// <summary>
        ///// METODO DE DETALHES DAS NOTIFICAÇÕES ENVIADAS
        ///// </summary>
        ///// <response code="200">Returns success</response>
        ///// <response code="400">Custom Error</response>
        ///// <response code="401">Unauthorize Error</response>
        ///// <response code="500">Exception Error</response>
        ///// <returns></returns>
        [HttpGet("DetailNotificationSended/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DetailNotificationSended([FromRoute] string id)
        {
            try
            {

                var entity = await _notificationSendedRepository.FindByIdAsync(id).ConfigureAwait(false);
                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(nameof(DefaultMessages.InformativeNotFound)));

                var informativeViewModel = _mapper.Map<NotificationSendedListViewModel>(entity);

                if (informativeViewModel.FamilyId != null && informativeViewModel.FamilyId.Count() > 0)
                {
                    var listFamily = await _familyRepository.FindIn("_id", informativeViewModel.FamilyId.Select(ObjectId.Parse).ToList());

                    if (listFamily.Count() > 0)
                        informativeViewModel.Family = _mapper.Map<List<FamilyHolderViewModel>>(listFamily.Select(x => x.Holder).ToList());
                }

                return Ok(Utilities.ReturnSuccess(data: informativeViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }
        ///// <summary>
        ///// LISTA TODAS AS NOTIFICAÇÕES
        ///// </summary>
        ///// <response code="200">Returns success</response>
        ///// <response code="400">Custom Error</response>
        ///// <response code="401">Unauthorize Error</response>
        ///// <response code="500">Exception Error</response>
        ///// <returns></returns>
        [HttpGet("GetAllNotification")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotification()
        {
            try
            {
                var entity = await _notificationRepository.FindByAsync(x => x.DataBlocked == null).ConfigureAwait(false);
                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(nameof(DefaultMessages.NotificationNotFound)));

                var list = _mapper.Map<List<NotificationListViewModel>>(entity);

                return Ok(Utilities.ReturnSuccess(data: list));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }

        /// <summary>
        /// Bbtenha apenas o contador de notificações por família
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetCountByFamily")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(NotificationListViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCountByFamily([FromQuery] bool archived, [FromQuery] bool setRead, [FromQuery] bool onlyNews)
        {
            try
            {
                var familyId = Request.GetUserId();

                int notificationCount = archived
                    ? await _notificationRepository.CountAsync(
                        x => x.Created <= DateTimeOffset.Now.AddDays(-30).ToUnixTimeSeconds() &&
                            (onlyNews ? x.DateViewed == null : x.DateViewed != null) &&
                            x.FamilyId == familyId).ConfigureAwait(false)
                    : await _notificationRepository.CountAsync(
                        x => x.Created > DateTimeOffset.Now.AddDays(-30).ToUnixTimeSeconds() &&
                            (onlyNews ? x.DateViewed == null : x.DateViewed != null) &&
                            x.FamilyId == familyId).ConfigureAwait(false);
                return Ok(Utilities.ReturnSuccess(data: notificationCount));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }

        /// <summary>
        /// LISTA TODOS OS NOTIFICAÇÕES POR FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetAllNotificationByFamily")]
        [HttpGet("GetAllNotificationByFamily/{page}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(NotificationListViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotificationByFamily([FromRoute] int page, [FromQuery] bool archived, [FromQuery] bool setRead, [FromQuery] int limit = 15)
        {
            IEnumerable<Notification> listNotification = null;
            var viewViewModel = new List<NotificationListViewModel>();
            try
            {
                var familyId = Request.GetUserId();

                if (page > 0)
                {
                    listNotification =
                    archived
                    ? await _notificationRepository.FindByAsync(x => x.Created <= DateTimeOffset.Now.AddDays(-30).ToUnixTimeSeconds() && x.FamilyId == familyId, page, Builders<Notification>.Sort.Descending(x => x.Created), limit).ConfigureAwait(false)
                    : await _notificationRepository.FindByAsync(x => x.Created > DateTimeOffset.Now.AddDays(-30).ToUnixTimeSeconds() && x.FamilyId == familyId, page, Builders<Notification>.Sort.Descending(x => x.Created), limit).ConfigureAwait(false);
                }
                else
                {
                    listNotification = await _notificationRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false);

                }
                if (listNotification.Count() == 0)
                    return Ok(Utilities.ReturnSuccess(data: viewViewModel));

                if (setRead)
                {
                    var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                    _notificationRepository.UpdateMultiple(Query<Notification>.In(x => x._id, listNotification.Select(x => x._id).ToList()), new UpdateBuilder<Notification>().Set(x => x.DateViewed, now), UpdateFlags.Multi);
                }

                viewViewModel = _mapper.Map<List<NotificationListViewModel>>(listNotification);

                return Ok(Utilities.ReturnSuccess(data: viewViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }

        /// <summary>
        /// CADASTRAR UMA NOTIFICAÇÃO POR FAMÍLIA OU TODAS
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        /// 
        ///         POST
        ///         {
        ///
        ///              "familyId": [
        ///                 "string"
        ///              ]
        ///              },
        ///              "allFamily": "false",  // INFORME TRUE PARA TODAS AS FAMILIAS
        ///              "image": "210831110311.jpg",
        ///              "title": "string",
        ///              "description": "string"
        ///          }    
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] NotificationViewModel model)
        {
            try
            {
                model.TrimStringProperties();
                var ignoreValidation = new List<string>();

                if (model.AllFamily)
                    ignoreValidation.Add(nameof(model.FamilyId));

                var isInvalidState = ModelState.ValidModelState(ignoreValidation.ToArray());

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);


                var listFamily = new List<Family>();
                if (model.AllFamily == true)
                {
                    listFamily = await _familyRepository.FindByAsync(x => x.Disabled == null).ConfigureAwait(false) as List<Family>;

                    model.FamilyId = listFamily.Select(x => x._id.ToString()).ToList();
                }
                else
                {
                    listFamily = await _familyRepository.FindIn("_id", model.FamilyId.Select(x => ObjectId.Parse(x)).ToList()) as List<Family>;
                    if (model.FamilyId.Count() != listFamily.Count())
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));
                }

                var entityNotification = new NotificationSended()
                {
                    Created = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    FamilyId = model.FamilyId,
                    Title = model.Title,
                    Description = model.Description,
                    Image = model.Image,
                    AllFamily = model.AllFamily
                };

                var informativeId = await _notificationSendedRepository.CreateAsync(entityNotification).ConfigureAwait(false);

                var listNotification = new List<Notification>();
                for (int i = 0; i < model.FamilyId.Count(); i++)
                {
                    listNotification.Add(new Notification()
                    {
                        Title = model.Title,
                        FamilyId = model.FamilyId[i],
                        Description = model.Description,
                        Image = model.Image
                    });
                }

                const int limit = 500;
                var registred = 0;
                var index = 0;

                while (listNotification.Count() > registred)
                {
                    var itensToRegister = listNotification.Skip(limit * index).Take(limit).ToList();

                    if (itensToRegister.Count() > 0)
                        await _notificationRepository.CreateAsync(itensToRegister);
                    registred += limit;
                    index++;
                }

                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Cadastrou uma notificação {Request.GetUserName()}", Request.GetUserId(), Request.GetUserName()?.Value, informativeId);

                return Ok(Utilities.ReturnSuccess(nameof(DefaultMessages.Registred)));

            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar nova notificação", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }

        /// <summary>
        /// MODIFICA UMA NOTIFICAÇÃO DE APP PARA LIDO
        /// </summary>
        /// <remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ChangeToRead/{notificationId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangeToRead(string notificationId)
        {
            try
            {

                var notificationEntity = await _notificationRepository.FindByIdAsync(notificationId).ConfigureAwait(false);
                if (notificationEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.NotificationNotFound));

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                notificationEntity.DateViewed = now;

                await _notificationRepository.UpdateAsync(notificationEntity).ConfigureAwait(false);
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Change, TypeResposible.UserAdminstratorGestor, $"Leu a notificação{Request.GetUserName()}", Request.GetUserId(), Request.GetUserName()?.Value, notificationId);
                return Ok(Utilities.ReturnSuccess(DefaultMessages.Updated));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi ler a notificação", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }

        /// <summary>
        /// MODIFICA UMA NOTIFICAÇÃO DE INFORMATIVOS PARA LIDO
        /// </summary>
        /// <remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ChangeToReadInfo/{notificationId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangeToReadInfo(string notificationId)
        {
            try
            {

                var entityNotification = await _notificationRepository.FindByIdAsync(notificationId).ConfigureAwait(false);
                if (entityNotification == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.NotificationNotFound));

                entityNotification.DateViewed = Utilities.ToTimeStamp(DateTime.Now);
                var entityFamily = await _notificationRepository.UpdateOneAsync(entityNotification).ConfigureAwait(false);
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Change, TypeResposible.UserAdminstratorGestor, $"Leu a notificação{Request.GetUserName()}", "", "", entityFamily);
                return Ok(Utilities.ReturnSuccess(DefaultMessages.Updated));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi ler a notificação", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }

        /// <summary>
        /// METODO DE LISTAGEM COM DATATABLE
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] long? startDate, [FromForm] long? endDate, [FromForm] bool forGestor, [FromForm] bool forTTS, [FromForm] bool forAdministrator,  [FromForm] bool onlyNoRead, [FromForm] bool setRead)
        {
            var response = new DtResult<NotificationViewModel>();
            try
            {
                var builder = Builders<Notification>.Filter;
                var conditions = new List<FilterDefinition<Notification>>();

                conditions.Add(builder.Where(x => x.Disabled == null));

                if (onlyNoRead)
                    conditions.Add(builder.Where(x => x.DateViewed == null));

                if (forGestor)
                    conditions.Add(builder.Where(x => x.FamilyId == null && (x.For == null || x.For == ForType.Gestor)));

                if (forTTS)
                    conditions.Add(builder.Where(x => x.FamilyId == null && (x.For == null || x.For == ForType.TTS)));


                // Administrator see a tts notifications too
                if (forAdministrator)
                    conditions.Add(builder.Where(x => x.FamilyId == null && (x.For == null || x.For == ForType.Administrador || x.For == ForType.TTS)));

                if (startDate != null)
                    conditions.Add(builder.Gte(x => x.Created, startDate));

                if (endDate != null)
                    conditions.Add(builder.Lte(x => x.Created, endDate));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = model.SortOrder;
                var totalRecords = (int)await _notificationRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                   ? Builders<Notification>.Sort.Descending(sortColumn)
                   : Builders<Notification>.Sort.Ascending(sortColumn);

                var retorno = await _notificationRepository
                 .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, fields: columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                   ? (int)await _notificationRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                   : totalRecords;

                response.Data = _mapper.Map<List<NotificationViewModel>>(retorno);
                response.Draw = model.Draw;
                response.RecordsFiltered = totalrecordsFiltered;
                response.RecordsTotal = totalRecords;

                if (setRead && retorno.Count() > 0)
                {
                    var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                    _notificationRepository.UpdateMultiple(Query<Notification>.In(x => x._id, retorno.Select(x => x._id).ToList()), new UpdateBuilder<Notification>().Set(x => x.DateViewed, now), UpdateFlags.Multi);
                }

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Erro = true;
                response.MessageEx = $"{ex.InnerException} {ex.Message}".Trim();
                return Ok(response);
            }
        }

        /// <summary>
        /// Count a notifications
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("CountNotifications")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CountNotifications([FromForm] DtParameters model, [FromForm] long? startDate, [FromForm] long? endDate, [FromForm] bool forGestor, [FromForm] bool forTTS, [FromForm] bool forAdministrator, [FromForm] bool onlyNoRead, [FromForm] string userId)
        {
            try
            {
                var builder = Builders<Notification>.Filter;
                var conditions = new List<FilterDefinition<Notification>>();

                conditions.Add(builder.Where(x => x.Disabled == null));

                conditions.Add(builder.Where(x => x.SeenBy  == null || (x.SeenBy != null && !x.SeenBy.Contains(userId))));

                if (forGestor)
                    conditions.Add(builder.Where(x => x.FamilyId == null && (x.For == null || x.For == ForType.Gestor)));

                if (forTTS)
                    conditions.Add(builder.Where(x => x.FamilyId == null && (x.For == null || x.For == ForType.TTS)));

                // Administrator see a tts notifications too
                if (forAdministrator)
                    conditions.Add(builder.Where(x => x.FamilyId == null && (x.For == null || x.For == ForType.Administrador || x.For == ForType.TTS)));

                if (startDate != null)
                    conditions.Add(builder.Gte(x => x.Created, startDate));

                if (endDate != null)
                    conditions.Add(builder.Lte(x => x.Created, endDate));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = model.SortOrder;
                var totalRecords = (int)await _notificationRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                   ? Builders<Notification>.Sort.Descending(sortColumn)
                   : Builders<Notification>.Sort.Ascending(sortColumn);


                return Ok(Utilities.ReturnSuccess(data: totalRecords));


            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }


        /// <summary>
        /// METODO DE LISTAGEM COM DATATABLE DAS NOTIFICAÇÕES ENVIADAS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("InformativeSended/LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(NotificationSendedListViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadDataNotificationSended([FromForm] DtParameters model, [FromForm] long? startDate, [FromForm] long? endDate)
        {
            var response = new DtResult<NotificationSendedListViewModel>();
            try
            {
                var builder = Builders<NotificationSended>.Filter;
                var conditions = new List<FilterDefinition<NotificationSended>>();

                conditions.Add(builder.Where(x => x.Created != null));

                if (startDate != null)
                    conditions.Add(builder.Gte(x => x.Created, startDate));

                if (endDate != null)
                    conditions.Add(builder.Lte(x => x.Created, endDate));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = model.SortOrder;
                var totalRecords = (int)await _notificationSendedRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                   ? Builders<NotificationSended>.Sort.Descending(sortColumn)
                   : Builders<NotificationSended>.Sort.Ascending(sortColumn);

                var retorno = await _notificationSendedRepository
                 .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, fields: columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                   ? (int)await _notificationSendedRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                   : totalRecords;

                response.Data = _mapper.Map<List<NotificationSendedListViewModel>>(retorno);
                response.Draw = model.Draw;
                response.RecordsFiltered = totalrecordsFiltered;
                response.RecordsTotal = totalRecords;

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Erro = true;
                response.MessageEx = $"{ex.InnerException} {ex.Message}".Trim();
                return Ok(response);
            }
        }
        /// <summary>
        /// METODO DE REMOVER ITEM
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Delete/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                if (!await _notificationRepository.CheckByAsync(x => x._id == ObjectId.Parse(id)).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InformativeNotFound));

                await _notificationRepository.DeleteOneAsync(id).ConfigureAwait(false);
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Delete, TypeResposible.UserAdminstratorGestor, $"Deletou uma notrificação {Request.GetUserName()}", Request.GetUserId(), Request.GetUserName()?.Value, id);
                return Ok(Utilities.ReturnSuccess(nameof(DefaultMessages.Deleted)));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Delete, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar nova notificação", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro(nameof(DefaultMessages.MessageException)));
            }
        }
        /// <summary>
        /// BLOQUEIA OU DESBLOQUEIA UMA NOTIFICAÇÃO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("BlockUnblock")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BlockUnblock([FromBody] BlockViewModel model)
        {

            try
            {
                if (ObjectId.TryParse(model.TargetId, out var unused) == false)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidIdentifier));

                var entityVideo = await _notificationRepository.FindOneByAsync(x => x._id == ObjectId.Parse(model.TargetId)).ConfigureAwait(false);

                if (entityVideo == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.VideoNotFound));

                entityVideo.DataBlocked = model.Block ? DateTimeOffset.Now.ToUnixTimeSeconds() : (long?)null;


                var entityId = await _notificationRepository.UpdateOneAsync(entityVideo).ConfigureAwait(false);
                await _utilService.RegisterLogAction(LocalAction.Notificacao, model.Block == true ? TypeAction.Block : TypeAction.UnBlock, TypeResposible.UserAdminstratorGestor, $"Bloqueou uma notificação {Request.GetUserName()}", Request.GetUserId(), Request.GetUserName()?.Value, entityVideo._id.ToString());
                return Ok(Utilities.ReturnSuccess(model.Block ? "Bloqueado com sucesso" : "Desbloqueado com sucesso"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Notificacao, TypeAction.Block, TypeResposible.UserAdminstratorGestor, $"Não foi bloquear a notificação", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }


    }

}