using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moralar.Data.Entities;
using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Course;
using Moralar.Domain.ViewModels.Family;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Services.Core.Interface;
using Moralar.WebApi.Services;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.Domain.Services;


namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class CourseController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationSendedRepository _notificationSendedRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly ICourseFamilyRepository _courseFamilyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilService _utilService;
        private readonly ISenderMailService _senderMailService;
        private readonly ISenderNotificationService _senderNotificationService;

        public CourseController(IMapper mapper, ICourseRepository courseRepository, INotificationRepository notificationRepository, INotificationSendedRepository notificationSendedRepository, IFamilyRepository familyRepository, ICourseFamilyRepository courseFamilyRepository, IHttpContextAccessor httpContextAccessor, IUtilService utilService, ISenderMailService senderMailService, ISenderNotificationService senderNotificationService)
        {
            _mapper = mapper;
            _courseRepository = courseRepository;
            _notificationRepository = notificationRepository;
            _notificationSendedRepository = notificationSendedRepository;
            _familyRepository = familyRepository;
            _courseFamilyRepository = courseFamilyRepository;
            _httpContextAccessor = httpContextAccessor;
            _utilService = utilService;
            _senderMailService = senderMailService;
            _senderNotificationService = senderNotificationService;
        }
        /// <summary>
        /// BLOQUEAR / DESBLOQUEAR CURSO 
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "id": "string", // required
        ///              "block": true,
        ///              "reason": "" //motivo de bloquear o usuário
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("BlockUnBlock")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BlockUnBlock([FromBody] BlockViewModel model)
        {
            try
            {
                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.TargetId));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entity = await _courseRepository.FindByIdAsync(model.TargetId);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));
                entity.DataBlocked = model.Block ? DateTimeOffset.Now.ToUnixTimeSeconds() : (long?)null;

                await _courseRepository.UpdateAsync(entity);
                var typeAction = model.Block == true ? TypeAction.Block : TypeAction.UnBlock;
                await _utilService.RegisterLogAction(LocalAction.Curso, typeAction, TypeResposible.UserAdminstratorGestor, $"Bloqueou/Desbloqueu o curso {entity.Title}", Request.GetUserId(), Request.GetUserName()?.Value, model.TargetId);
                return Ok(Utilities.ReturnSuccess(model.Block ? "Bloqueado com sucesso" : "Desbloqueado com sucesso"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Familia, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível bloquer Família", Request.GetUserId(), Request.GetUserName()?.Value, model.TargetId, "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTAGEM DOS CURSOS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(List<CourseListViewModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] string title, [FromForm] long? startDate, [FromForm] long? endDate)
        {
            var response = new DtResult<CourseListViewModel>();
            var listCourseFamily = new List<CourseFamily>();
            var listFamily = new List<Family>();
            try
            {
                var builder = Builders<Data.Entities.Course>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.Course>>();

                conditions.Add(builder.Where(x => x.Created != null));

                if (string.IsNullOrEmpty(title) == false)
                    conditions.Add(builder.Regex(x => x.Title, new BsonRegularExpression(new Regex(title, RegexOptions.IgnoreCase))));

                if (startDate != null)
                    conditions.Add(builder.Where(x => x.Created >= startDate));

                if (endDate != null)
                    conditions.Add(builder.Where(x => x.Created <= endDate));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _courseRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.Course>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.Course>.Sort.Ascending(sortColumn);

                var retorno = await _courseRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, columns) as List<Course>;

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                    ? (int)await _courseRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                    : totalRecords;

                if (retorno.Count() > 0)  
                   listCourseFamily = await _courseFamilyRepository.FindIn(nameof(CourseFamily.CourseId), retorno.Select(x => x._id.ToString()).ToList()) as List<CourseFamily>;
               
                var viewModelData = _mapper.Map<List<Course>, List<CourseListViewModel>>(retorno, opt => opt.AfterMap(async (src, dest) =>
                {
                    for (int i = 0; i < src.Count(); i++)
                    {
                        
                        dest[i].TotalInscriptions = listCourseFamily.Count(x => x.TypeStatusCourse == TypeStatusCourse.Inscrito && x.CourseId == src[i]._id.ToString());
                        dest[i].TotalWaitingList = listCourseFamily.Count(x => x.TypeStatusCourse == TypeStatusCourse.ListaEspera && x.CourseId == src[i]._id.ToString());
                    }

                }));

                response.Data = viewModelData;
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
        /// DETALHES DO CURSO 
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         GET
        ///             {
        ///              "id": "string", // required
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("Detail/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Detail([FromRoute] string id)
        {
            var listFamily = new List<Family>();
            try
            {
                var userId = Request.GetUserId();

                var entity = await _courseRepository.FindByIdAsync(id).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseNotFound));

                var viewmodelData = _mapper.Map<CourseViewModel>(entity);                

                var builder = Builders<CourseFamily>.Filter;
                var conditions = new List<FilterDefinition<CourseFamily>>();


                /* Verifica se a família já é inscrita no curso */
                viewmodelData.IsSubscribed = await _courseFamilyRepository.CheckByAsync(x => x.FamilyId == userId && x.CourseId == id);
                
                viewmodelData.TotalSubscribers = await _courseFamilyRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(builder.Eq("CourseId", id), builder.Eq(x => x.TypeStatusCourse, TypeStatusCourse.Inscrito)));
                
                viewmodelData.TotalWaitingList = await _courseFamilyRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(builder.Eq("CourseId", id), builder.Eq(x => x.TypeStatusCourse, TypeStatusCourse.ListaEspera)));

               
                conditions.Add(builder.Where(x => x.CourseId == id));

                var listsubscribers = await _courseFamilyRepository.GetCollectionAsync().FindSync(builder.And(conditions)).ToListAsync();

                var responseSubscibers = new List<FamilyCourseRegisteredViewModel>();
                var responseWaiting = new List<FamilyCourseWaitingViewModel>();                

                for (int i = 0; i < listsubscribers.Count; i++)
                {
                    var item = listsubscribers[i];                  
                   
                    var familyEntity = await _familyRepository.FindByIdAsync(item.FamilyId);

                    if (familyEntity != null)
                    {
                        if (item.TypeStatusCourse == TypeStatusCourse.Inscrito)
                        {
                            responseSubscibers.Add(new FamilyCourseRegisteredViewModel()
                            {

                                Number = familyEntity.Holder.Number,
                                Name = familyEntity.Holder.Name,
                                Cpf = familyEntity.Holder.Cpf,
                                TypeStatusCourse = TypeStatusCourse.Inscrito

                            }); ;
                            viewmodelData.ListSubscribers = responseSubscibers;
                        }
                        else
                        {
                            responseWaiting.Add(new FamilyCourseWaitingViewModel()
                            {

                                Number = familyEntity.Holder.Number,
                                Name = familyEntity.Holder.Name,
                                Cpf = familyEntity.Holder.Cpf,
                                TypeStatusCourse = TypeStatusCourse.ListaEspera

                            });
                            viewmodelData.ListWaitingList = responseWaiting;
                        }
                    }
                }

                return Ok(Utilities.ReturnSuccess(data: viewmodelData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LISTA TODOS OS CURSOS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                var entity = await _courseRepository.FindByAsync(x => x.EndDate > now && x.DataBlocked == null, Builders<Course>.Sort.Descending(x => x.StartDate)).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseNotFound));

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<CourseViewModel>>(entity)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LISTA OS CURSOS POR FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetCourseByFamilyId")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCourseByFamilyId([FromQuery] string familyId)
        {
            try
            {
                var entityFamily = await _familyRepository.FindByIdAsync(familyId).ConfigureAwait(false);

                if (entityFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));


                var entityCourseFamily = await _courseFamilyRepository.FindByAsync(x => x.FamilyId == entityFamily._id.ToString()).ConfigureAwait(false) as List<CourseFamily>;

                if (entityCourseFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseNotFound));


                var entityCourse = await _courseRepository.FindIn("_id", entityCourseFamily.Select(x => ObjectId.Parse(x.CourseId)).ToList()) as List<Course>;

                if (entityCourse == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseNotFound));



                var viewModelData = _mapper.Map<List<CourseFamilyListViewModel>>(entityCourse);


                for (int i = 0; i < viewModelData.Count(); i++)
                {
                    var familyCourse = entityCourseFamily.Find(x => x.CourseId == viewModelData[i].Id.ToString());
                    if (familyCourse != null)
                        viewModelData[i].TypeStatusCourse = familyCourse.TypeStatusCourse;
                }



                return Ok(Utilities.ReturnSuccess(data: viewModelData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }

        /// <summary>
        /// REGISTRAR UM NOVO CURSO
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        ///        POST
        ///        {
        ///          "title": "string",
        ///          "img": "string",
        ///          "startDate": 0,
        ///          "endDate": 0,
        ///          "schedule": "string",
        ///          "place": "string",
        ///          "workLoad": "string",
        ///          "description": "string",
        ///          "startTargetAudienceAge": 0,
        ///          "endTargetAudienceAge": 0,
        ///          "typeGenre": "Feminino",
        ///          "numberOfVacancies": 0,
        ///          "id": "string"
        ///        }
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
        public async Task<IActionResult> Register([FromBody] CourseViewModel model)
        {
            //var claim = Util.SetRole(TypeProfile.Profile);
            //var typeAction = string.IsNullOrEmpty(model.Id) ? TypeAction.Register : TypeAction.Change;
            try
            {
                model.TrimStringProperties();
                var ignoreValidation = new List<string>();
                var isInvalidState = ModelState.ValidModelState(ignoreValidation.ToArray());

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entity = _mapper.Map<Data.Entities.Course>(model);
                entity.Img = model.Img.SetPathImage();
                var entityId = await _courseRepository.CreateAsync(entity).ConfigureAwait(false);


                await _utilService.RegisterLogAction(LocalAction.Curso, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Cadastrou novo curso {model.Title}", Request.GetUserId(), Request.GetUserName()?.Value, entityId, "");

               var entityFamily = new List<Family>();
                
               entityFamily = await _familyRepository.FindByAsync(x => x.Disabled == null).ConfigureAwait(false) as List<Family>;

                var title = "Novo curso";
                var content = $"{entity.Title} foi disponibilizado para inscrição";

                var listNotification = new List<Notification>();
                for (int i = 0; i < entityFamily.Count(); i++)
                {
                    var familyItem = entityFamily[i];

                    listNotification.Add(new Notification()
                    {
                        For = ForType.Family,
                        FamilyId = familyItem._id.ToString(),
                        Title = title,
                        Description = content
                    });
                }

                await _notificationRepository.CreateAsync(listNotification);

                var allDeviceId = entityFamily.SelectMany(x => x.DeviceId).Distinct().Where(x => string.IsNullOrEmpty(x) == false).ToList();

                if (allDeviceId.Count > 0)
                {                    

                    dynamic payloadPush = Util.GetPayloadPush();   
                    dynamic settingPush = Util.GetSettingsPush();

                    await _senderNotificationService.SendPushAsync(title, content, allDeviceId, data: payloadPush, settings: settingPush, priority: 10);

                }

                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));

            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Curso, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar novo curso", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// REGISTRAR FAMÍLIA PARA O CURSO
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        ///         POST
        ///        {
        ///          "familyId": "string",
        ///          "courseId": "string",
        ///          "waitInTheQueue": true, AGUARDAR NA FILA DE ESPERA
        ///          "id": "string"
        ///        }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("RegisterFamilyToTrainning")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterFamilyToTrainning([FromBody] CourseFamilyViewModel model)
        {
            try
            {
                var entityFamily = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);
                if (entityFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                if (await _courseFamilyRepository.CheckByAsync(x => x.FamilyId == model.FamilyId && x.CourseId == model.CourseId).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro("Família já realizou o curso"));
                var entityCourse = await _courseRepository.FindByIdAsync(model.CourseId).ConfigureAwait(false);
                if (entityCourse == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseNotFound));

                var canRegister = Util.HasValidMember(entityFamily, entityCourse.StartTargetAudienceAge, entityCourse.EndTargetAudienceAge, entityCourse.TypeGenre);

                if (canRegister.Birthday == false)
                    return BadRequest(Utilities.ReturnErro("Não é possível realizar a inscrição neste curso, por não condizer com o público a que é destinado"));

                if (canRegister.Gender == false)
                    return BadRequest(Utilities.ReturnErro("Não é possível realizar a inscrição neste curso, por não condizer com o gênero que é destinado"));

                var qtdCourse = await _courseFamilyRepository.FindByAsync(x => x.CourseId == model.CourseId).ConfigureAwait(false) as List<CourseFamily>;
                if (qtdCourse.Count() >= entityCourse.NumberOfVacancies && model.WaitInTheQueue == false)
                    return BadRequest(Utilities.ReturnErro("Deseja aguardar na fila de espera?"));

                var entity = _mapper.Map<Data.Entities.CourseFamily>(model);
                
                entity.TypeStatusCourse = model.WaitInTheQueue == false ? TypeStatusCourse.Inscrito : TypeStatusCourse.ListaEspera;
                entity.HolderName = entityFamily.Holder.Name;
                
                var entityId = await _courseFamilyRepository.CreateAsync(entity).ConfigureAwait(false);


                await _notificationRepository.CreateAsync(new Notification
                {
                    Title = "Confirmação de inscrição em curso",
                    Description = $"Olá {  entityFamily.Holder.Name }!" +
                                  $"Sua inscrição no curso {  entityCourse.Title }" +
                                  "foi realizada com sucesso!Fique atento, seu curso começará em breve.",
                    FamilyId = entity._id.ToString(),
                });
                await _utilService.RegisterLogAction(LocalAction.Curso, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Cadastrou família  {entityFamily.Holder.Name} para fazer o curso", Request.GetUserId(), "", entityId, "");
                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Curso, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar nova Família", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// CANCELA CURSO DA FAMÍLIA
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        ///         POST
        ///        {
        ///        "familyId": "string",
        ///        "courseId": "string"
        ///        }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("CancelFamilyToTrainning")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CancelFamilyToTrainning([FromBody] CourseCancelViewModel model)
        {
            try
            {
                var entityFamily = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);
                if (entityFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var entityCourse = await _courseRepository.FindByIdAsync(model.CourseId).ConfigureAwait(false);
                if (entityCourse == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseNotFound));

                var courses = await _courseFamilyRepository.FindByAsync(x => x.CourseId == model.CourseId).ConfigureAwait(false);
                var entityCourseFamily = courses.Where(x => x.FamilyId == model.FamilyId && x.CourseId == model.CourseId).FirstOrDefault();
                if (entityCourseFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CourseFamilyNotFound));

                await _courseFamilyRepository.DeleteOneAsync(entityCourseFamily._id.ToString()).ConfigureAwait(false);


                if (courses.ToList().Count(x => x.TypeStatusCourse == TypeStatusCourse.ListaEspera) > 0)
                {
                    var familyToCourseAvaliable = courses.ToList().Where(x => x.TypeStatusCourse == TypeStatusCourse.ListaEspera).OrderBy(x => x.Created).FirstOrDefault();
                    if (familyToCourseAvaliable != null)
                    {
                        familyToCourseAvaliable.TypeStatusCourse = TypeStatusCourse.Inscrito;
                        await _courseFamilyRepository.UpdateOneAsync(familyToCourseAvaliable).ConfigureAwait(false);


                        await _notificationRepository.CreateAsync(new Notification
                        {
                            Title = "Liberação de vaga em curso",
                            Description = $"Olá {  entityFamily.Holder.Name }!" +
                                          $" Sua inscrição para o curso {  entityCourse.Title }" +
                                           " que estava aguardando na lista de espera foi confirmada com sucesso! Fique atento, seu curso começará em breve.",
                            FamilyId = entityFamily._id.ToString(),
                        });
                    }
                }
                await _notificationRepository.CreateAsync(new Notification
                {
                    Title = "Cancelamento de inscrição em curso",
                    Description = $"Olá {  entityFamily.Holder.Name }!" +
                                 $" Seu curso {  entityCourse.Title }" +
                                 " foi cancelado com sucesso!",
                    FamilyId = entityFamily._id.ToString(),
                });
                await _utilService.RegisterLogAction(LocalAction.Curso, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Cancelou o curso {entityFamily.Holder.Name} para fazer o curso", Request.GetUserId(), "", model.CourseId, "");
                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Curso, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar nova Família", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// EXPORTAR PARA EXCEL
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Export")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]


        public async Task<IActionResult> Export([FromForm] DtParameters model, [FromForm] string title, [FromForm] long? startDate, [FromForm] long? endDate)
        {
            var listCourseFamily = new List<CourseFamily>();
            try
            {
                var builder = Builders<Course>.Filter;
                var conditions = new List<FilterDefinition<Course>>();

                conditions.Add(builder.Where(x => x.Created != null));

                if (string.IsNullOrEmpty(title) == false)
                    conditions.Add(builder.Regex(x => x.Title, new BsonRegularExpression(new Regex(title, RegexOptions.IgnoreCase))));

                if (startDate != null)
                    conditions.Add(builder.Where(x => x.Created >= startDate));

                if (endDate != null)
                    conditions.Add(builder.Where(x => x.Created <= endDate));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _courseRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Course>.Sort.Descending(sortColumn)
                    : Builders<Course>.Sort.Ascending(sortColumn);

                var retorno = await _courseRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, totalRecords, conditions, columns) as List<Course>;

                //if (retorno.Count() > 0)
                    //listCourseFamily = await _courseFamilyRepository.FindIn(nameof(CourseFamily.CourseId), retorno.Select(x => x._id.ToString()).ToList()) as List<CourseFamily>;

                var listViewModel = _mapper.Map<List<CourseExportViewModel>>(retorno);

                if (retorno.Count() > 0)
                {
                    for (int i = 0; i < retorno.Count(); i++)
                    {
                        var item = retorno[i];

                        var familyInscriptions = await _courseFamilyRepository.FindByAsync(x => x.CourseId == item._id.ToString() && x.TypeStatusCourse == TypeStatusCourse.Inscrito);
                       
                        var familyWaiting = await _courseFamilyRepository.FindByAsync(x => x.CourseId == item._id.ToString() && x.TypeStatusCourse == TypeStatusCourse.ListaEspera);
                        
                        listViewModel[i].TotalInscriptions = familyInscriptions.Count();
                        listViewModel[i].TotalWaitingList = familyWaiting.Count(); 
                        listViewModel[i].FamilyNameInscriptions = string.Join(", ", familyInscriptions.Select(x => x.HolderName).ToList()).TrimEnd(',');
                        listViewModel[i].FamilyNameWaiting = string.Join(", ", familyWaiting.Select(x => x.HolderName).ToList()).TrimEnd(',');

                    }
                }        

                var fileName = "Cursos.xlsx";
                var path = UtilService.getFilePathFromServer("ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);

                Utilities.ExportToExcel(listViewModel, path, fileName: fileName.Split('.')[0]);
                if (System.IO.File.Exists(fullPathFile) == false)
                    return BadRequest(Utilities.ReturnErro("Ocorreu um erro fazer download do arquivo"));

                var fileBytes = System.IO.File.ReadAllBytes(fullPathFile);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(Utilities.ReturnErro(ex.Message));
            }
        }
    }
}