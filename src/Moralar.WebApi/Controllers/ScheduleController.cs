using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using MongoDB.Bson;
using MongoDB.Driver;

using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels.Property;
using Moralar.Domain.ViewModels.Schedule;
using Moralar.Domain.ViewModels.ScheduleHistory;
using Moralar.Repository.Interface;

using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;
using System.Text;
using Moralar.Domain.Services;

namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class ScheduleController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IFamilyRepository _familyRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleHistoryRepository _scheduleHistoryRepository;
        private readonly IUtilService _utilService;
        private readonly ISenderMailService _senderMailService;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizFamilyRepository _quizFamilyRepository;
        private readonly ICourseFamilyRepository _courseFamilyRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IPropertiesInterestRepository _propertiesInterestRepository;
        private readonly IResidencialPropertyRepository _residencialPropertyRepository;
        private readonly INotificationSendedRepository _notificationSendedRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ISenderNotificationService _senderNotificationService;

        public ScheduleController(IMapper mapper, IFamilyRepository familyRepository, IScheduleRepository scheduleRepository, IScheduleHistoryRepository scheduleHistoryRepository, IUtilService utilService, ISenderMailService senderMailService, IQuizRepository quizRepository, IQuizFamilyRepository quizFamilyRepository, ICourseFamilyRepository courseFamilyRepository, ICourseRepository courseRepository, IPropertiesInterestRepository propertiesInterestRepository, IResidencialPropertyRepository residencialPropertyRepository, INotificationSendedRepository notificationSendedRepository, INotificationRepository notificationRepository, ISenderNotificationService senderNotificationService, IProfileRepository profileRepository)
        {
            _mapper = mapper;
            _familyRepository = familyRepository;
            _scheduleRepository = scheduleRepository;
            _scheduleHistoryRepository = scheduleHistoryRepository;
            _utilService = utilService;
            _senderMailService = senderMailService;
            _quizRepository = quizRepository;
            _quizFamilyRepository = quizFamilyRepository;
            _courseFamilyRepository = courseFamilyRepository;
            _courseRepository = courseRepository;
            _propertiesInterestRepository = propertiesInterestRepository;
            _residencialPropertyRepository = residencialPropertyRepository;
            _notificationSendedRepository = notificationSendedRepository;
            _notificationRepository = notificationRepository;
            _senderNotificationService = senderNotificationService;
            _profileRepository = profileRepository;
        }




        /// <summary>
        /// RETORNA O HISTÓRICO DOS AGENDAMENTOS POR FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetHistoryByFamily/{familyId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetHistoryByFamily([FromRoute] string familyId)
        {
            try
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                var listSchedule = await _scheduleRepository.FindByAsync(x => (x.FamilyId == familyId && x.Date <= now && x.TypeScheduleStatus == TypeScheduleStatus.Finalizado) || (x.TypeScheduleStatus == TypeScheduleStatus.Reagendado), Builders<Schedule>.Sort.Ascending(x => x.TypeSubject).Descending(x => x.Created));

                // var listSchedule = await _scheduleHistoryRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false);

                // if (listSchedule == null)
                //     return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotFound));

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<ScheduleHistoryViewModel>>(listSchedule)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// RETORNA OS PRÓXIMOS AGENDAMENTOS POR FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetScheduleByFamily/{familyId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetScheduleByFamily([FromRoute] string familyId)
        {
            try
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                var builder = Builders<Schedule>.Filter;
                var conditions = new List<FilterDefinition<Schedule>>();

                conditions.Add(builder.Eq(x => x.FamilyId, familyId));

                var listSchedule = await _scheduleRepository.GetCollectionAsync().FindSync(builder.And(conditions), new FindOptions<Schedule>()
                {
                    Sort = Builders<Schedule>.Sort.Descending(x => x.Date),
                    Collation = new Collation("en", strength: CollationStrength.Primary)

                }).ToListAsync();

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<ScheduleListViewModel>>(listSchedule.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// RETORNA OS PRÓXIMOS AGENDAMENTOS POR FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetScheduleTimeLineByFamily/{familyId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetScheduleTimeLineByFamily([FromRoute] string familyId)
        {
            try
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                var builder = Builders<Schedule>.Filter;
                var conditions = new List<FilterDefinition<Schedule>>();

                var timeLineTypeSubject = Util.GetTypeSubjectTimeline();

                conditions.Add(builder.Eq(x => x.FamilyId, familyId));
                conditions.Add(builder.In(x => x.TypeSubject, timeLineTypeSubject));

                var listSchedule = await _scheduleRepository.GetCollectionAsync().FindSync(builder.And(conditions), new FindOptions<Schedule>()
                {
                    Sort = Builders<Schedule>.Sort.Descending(x => x.Date),
                    Collation = new Collation("en", strength: CollationStrength.Primary)

                }).ToListAsync();
                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<ScheduleListViewModel>>(listSchedule.OrderBy(x => x.Date).ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LISTAGEM DOS QUESTINÁRIOS DATATABLE
        /// </summary>
        /// <response code = "200" > Returns success</response>
        /// <response code = "400" > Custom Error</response>
        /// <response code = "401" > Unauthorize Error</response>
        /// <response code = "500" > Exception Error</response>
        /// <returns></returns>
        [HttpPost("LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] string number, [FromForm] string holderName, [FromForm] string holderCpf, [FromForm] long? startDate, [FromForm] long? endDate, [FromForm] string place, [FromForm] string description, [FromForm] TypeScheduleStatus? status, [FromForm] TypeSubject? type)
        {
            var response = new DtResult<ScheduleListViewModel>();
            try
            {
                var builder = Builders<Data.Entities.Schedule>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.Schedule>>();

                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null));

                if (!string.IsNullOrEmpty(number))
                    conditions.Add(builder.Where(x => x.HolderNumber == number));
                if (!string.IsNullOrEmpty(holderName))
                    conditions.Add(builder.Where(x => x.HolderName.ToUpper().Contains(holderName.ToUpper())));
                if (!string.IsNullOrEmpty(holderCpf))
                    conditions.Add(builder.Where(x => x.HolderCpf == holderCpf.OnlyNumbers()));
                if (startDate.HasValue)
                    conditions.Add(builder.Where(x => x.Date >= startDate));
                if (endDate.HasValue)
                    conditions.Add(builder.Where(x => x.Date <= endDate));
                if (!string.IsNullOrEmpty(place))
                    conditions.Add(builder.Where(x => x.Place.ToUpper().Contains(place.ToUpper())));
                if (!string.IsNullOrEmpty(description))
                    conditions.Add(builder.Where(x => x.Description.ToUpper().Contains(description.ToUpper())));

                if (status != null)
                    conditions.Add(builder.Where(x => x.TypeScheduleStatus == status));

                if (type != null)
                    conditions.Add(builder.Where(x => x.TypeSubject == type));


                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _scheduleRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.Schedule>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.Schedule>.Sort.Ascending(sortColumn);

                var retorno = await _scheduleRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                    ? (int)await _scheduleRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                    : totalRecords;

                response.Data = _mapper.Map<List<ScheduleListViewModel>>(retorno);
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
        /// DETALHES DO QUIZ
        /// </summary>
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
            try
            {


                var entity = await _scheduleRepository.FindByIdAsync(id).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<ScheduleListViewModel>(entity)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// REGISTRAR UM NOVO AGENDAMENTO
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        ///        POST
        ///        {
        ///             "date": 0,
        ///             "place": "string",
        ///             "description": "string",
        ///             "familyId": "string",
        ///             "typeSubject": " Visita do TTS",
        ///             "quiz": { "id":"","title":""},
        ///             "id": "string"
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
        public async Task<IActionResult> Register([FromBody] ScheduleRegisterViewModel model)
        {

            try
            {


                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Date), nameof(model.Description), nameof(model.FamilyId), nameof(model.Place));
                if (isInvalidState != null)
                    return BadRequest(isInvalidState);


                var dateToSchedule = Utilities.TimeStampToDateTime(model.Date);
                if (dateToSchedule < DateTime.Now)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.DateInvalidToSchedule));

                var familyEntity = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);

                if (familyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var schedule = await _scheduleRepository.FindByAsync(x => x.FamilyId == model.FamilyId && x.TypeSubject == TypeSubject.ReuniaoPGM);

               /*if (model.TypeSubject != TypeSubject.ReuniaoPGM)
                {
                    // Checa se as etapas anteriores foram cumplidas
                    if (schedule.Count() == 0)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.StageInvalidToSchedule));

                    if (schedule.Count(x => (x.TypeScheduleStatus == TypeScheduleStatus.Confirmado || x.TypeScheduleStatus == TypeScheduleStatus.AguardandoConfirmacao)) > 0)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.StageInvalidToSchedule));
                }
            */

                var scheduleEntity = _mapper.Map<Schedule>(model);

                scheduleEntity.HolderCpf = familyEntity.Holder.Cpf;
                scheduleEntity.HolderName = familyEntity.Holder.Name;
                scheduleEntity.HolderNumber = familyEntity.Holder.Number;
                scheduleEntity.TypeSubject = model.TypeSubject;
                scheduleEntity.TypeScheduleStatus = TypeScheduleStatus.AguardandoConfirmacao;

                var scheduleId = await _scheduleRepository.CreateAsync(scheduleEntity).ConfigureAwait(false);

                var scheduleHistoryEntity = _mapper.Map<ScheduleHistory>(model);

                scheduleHistoryEntity.HolderCpf = familyEntity.Holder.Cpf;
                scheduleHistoryEntity.HolderName = familyEntity.Holder.Name;
                scheduleHistoryEntity.HolderNumber = familyEntity.Holder.Number;
                scheduleHistoryEntity.TypeSubject = model.TypeSubject;
                scheduleHistoryEntity.ScheduleId = scheduleId;

                await _scheduleHistoryRepository.CreateAsync(scheduleHistoryEntity).ConfigureAwait(false);

                /*GERAR OS PASSOS DE DE LINHA DO TEMPO*/
                if (model.TypeSubject == TypeSubject.ReuniaoPGM)
                {
                    scheduleHistoryEntity._id = new ObjectId();
                    scheduleHistoryEntity.TypeSubject = TypeSubject.EscolhaDoImovel;
                    await _scheduleHistoryRepository.CreateAsync(scheduleHistoryEntity).ConfigureAwait(false);

                    scheduleHistoryEntity._id = new ObjectId();
                    scheduleHistoryEntity.TypeSubject = TypeSubject.Mudanca;
                    await _scheduleHistoryRepository.CreateAsync(scheduleHistoryEntity).ConfigureAwait(false);

                    scheduleHistoryEntity._id = new ObjectId();
                    scheduleHistoryEntity.TypeSubject = TypeSubject.AcompanhamentoPosMudança;
                    await _scheduleHistoryRepository.CreateAsync(scheduleHistoryEntity).ConfigureAwait(false);
                }

                var dataBody = Util.GetTemplateVariables();

                string title = $"Olá {familyEntity.Holder.Name.GetFirstName()}!";
                string messageBody = null;



                switch (model.TypeSubject)
                {
                    case TypeSubject.VisitaTTS:

                        messageBody = $"<p>A visita com a equipa tts já está agendada.<br/><br/>"
                                    + $"Dia {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):dd/MM/yyyy}, horário {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):HH:mm}, endereço {model.Place}.</p>"
                                    + $"<p>Até lá!.</p>";
                        break;
                    case TypeSubject.ReuniaoTTS:

                        messageBody = $"<p>A reunião com a equipe tts já foi agendada.<br/><br/>"
                                    + $"Dia {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):dd/MM/yyyy}, horário {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):HH:mm}, endereço {model.Place}.</p>"
                                    + $"<p>Até lá!.</p>";
                        break;
                    case TypeSubject.ReuniaoPGM:
                        title = "Cadastro de Agendamento";
                        messageBody = $"<p>Olá {familyEntity.Holder.Name.GetFirstName()}.<br/><br/>"
                                    + $"Foi cadastrado um agendamento para o dia {dateToSchedule:dd/MM/yyyy HH:mm}.</p>";
                        break;

                    case TypeSubject.VisitaImovel:

                        messageBody = $"<p>Sua visita domiciliar foi marcada.<br/><br/>"
                                    + $"Dia {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):dd/MM/yyyy}, horário {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):HH:mm}, endereço {model.Place}.</p>"
                                    + $"<p>Até lá!.</p>";
                        break;
                    case TypeSubject.EscolhaDoImovel:

                        messageBody = $"<p>Notificamos que sua escolha de imóvel foi bem sucedida.<br/><br/>"
                                    + $"Caso queira acompanhar, entre em contato com a equipe do TTS.</p>";
                        break;
                    case TypeSubject.Demolicão:

                        messageBody = $"<p>Notificamos que temos agendada a demolição que foi acordada com a equipa tts.<br/><br/>"
                                    + $"Caso pretenda saber mais sobre o assunto contacte a equipa tts.</p>";
                        break;
                    case TypeSubject.Outros:

                        messageBody = $"<p>Foi agendada uma visita domiciliária, caso pretenda saber mais sobre o assunto contacte a equipa tts.<br/><br/>"
                                    + $"Dia {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):dd/MM/yyyy}, horário {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):HH:mm}.<br/>"
                                     + $"<p>Até lá!.</p>";
                        break;
                    case TypeSubject.Mudanca:

                        messageBody = $"<p>Sua mudança foi agendada.<br/><br/>"
                                    + $"Dia {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):dd/MM/yyyy}, horário {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):HH:mm}.<br/>"
                                    + $"Lembre-se: todos seus pertences devem estar embalados e o imóvel vazio.<br/>"
                                    + $"Fique tranquilo, tudo correrá bem!.</p>";
                        break;
                    case TypeSubject.AcompanhamentoPosMudança:

                        messageBody = $"<p>Sua acompanhamento PosMudança foi agendada.<br/>"
                                    + $"Dia {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):dd/MM/yyyy}, horário {Utilities.TimeStampToDateTimeLocalZone(scheduleEntity.Date):HH:mm}.<br/>"
                                    + $"<p>Até lá!.</p>";

                        break;

                    default:
                        break;
                }

                await _utilService.SendNotify(title, messageBody, familyEntity.Holder.Email, familyEntity.DeviceId, ForType.Family, model.FamilyId, null, null,
                    UtilService.getModuleSchedule(model.TypeScheduleStatus, scheduleEntity._id.ToString()));

                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// GERAR AGENDAMENTO DE ESCOLHA DE IMÓVEL
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "id":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ToStageChosenProperty")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ToStageChosenProperty([FromBody] BaseViewModel model)
        {
            try
            {

                var familyEntity = await _familyRepository.FindByIdAsync(model.Id);

                if (familyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var scheduleEntity = await _scheduleRepository.FindOneByAsync(x => x.FamilyId == model.Id && x.TypeSubject == TypeSubject.EscolhaDoImovel);

                if (scheduleEntity == null)
                {

                    var message = new StringBuilder();
                    message.AppendLine($"<p>Olá {familyEntity.Holder.Name}<br/>");
                    message.AppendLine($"Agora você já pode escolher seus imóveis de interesse</p>");

                    var title = "Escolha de imóveis liberada";

                    var content = "Verifique a area e escolha de imóveis";
                    var now = new DateTimeOffset(DateTime.Today.AddDays(15)).ToUnixTimeSeconds();

                    scheduleEntity = new Schedule()
                    {
                        TypeSubject = TypeSubject.EscolhaDoImovel,
                        FamilyId = model.Id,
                        Description = "Escolha de imóveis de interesse",
                        HolderCpf = familyEntity.Holder.Cpf,
                        HolderName = familyEntity.Holder.Name,
                        HolderNumber = familyEntity.Holder.Number,
                        TypeScheduleStatus = TypeScheduleStatus.Confirmado,
                        Place = "APP",
                        Date = now
                    };

                    await _scheduleRepository.CreateAsync(scheduleEntity);
                    await _utilService.SendNotify(title, message.ToString(), familyEntity.Holder.Email, familyEntity.DeviceId, familyId: model.Id, contentPush: content);
                }

                return Ok(Utilities.ReturnSuccess(DefaultMessages.SelectPropertyAvailable, DefaultMessages.SelectPropertyAvailable));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// REGISTRAR UM NOVO QUESTIONÁRIO
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        ///        POST
        ///        {
        ///          "date": 0,
        ///          "place": "string",
        ///          "description": "string",
        ///          "familyId": "string",
        ///          "typeSubject": " Visita do TTS",
        ///          "id": "string"
        ///        }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("RegisterResettlement")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterResettlement([FromBody] ScheduleRegisterViewModel model)
        {

            try
            {

                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Date), nameof(model.Description), nameof(model.FamilyId), nameof(model.Place));
                if (isInvalidState != null)
                    return BadRequest(isInvalidState);


                var dateToSchedule = Utilities.TimeStampToDateTime(model.Date);
                if (dateToSchedule < DateTime.Now)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.DateInvalidToSchedule));

                var family = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));


                var scheduleEntity = _mapper.Map<Schedule>(model);
                scheduleEntity.HolderCpf = family.Holder.Cpf;
                scheduleEntity.HolderName = family.Holder.Name;
                scheduleEntity.HolderNumber = family.Holder.Number;
                scheduleEntity.TypeSubject = TypeSubject.ReuniaoPGM;
                scheduleEntity.TypeScheduleStatus = TypeScheduleStatus.AguardandoConfirmacao;
                await _scheduleRepository.CreateAsync(scheduleEntity).ConfigureAwait(false);


                var dataBody = Util.GetTemplateVariables();
                dataBody.Add("{{ title }}", "Cadastro de Agendamento");
                dataBody.Add("{{ message }}", $"<p>Olá {family.Holder.Name.GetFirstName()}<br/>" +
                                              $"<p>Foi cadastrado um agendamento para o horário {dateToSchedule.ToString("dd/MM/yyyy HH:mm")}</p>"
                                            );

                var body = _senderMailService.GerateBody("custom", dataBody);

                await _senderMailService.SendMessageEmailAsync("Moralar", family.Holder.Email, body, "Cadastro de Agendamento");
                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }



        /// <summary>
        /// RETORNA OS  DETALHES DA LINHA DO TEMPO DA FAMÍLIA - ESCOLHA DO IMÓVEL
        /// </summary>
        /// <response code = "200" > Returns success</response>
        /// <response code = "400" > Custom Error</response>
        /// <response code = "401" > Unauthorize Error</response>
        /// <response code = "500" > Exception Error</response>
        /// <returns></returns>
        [HttpGet("DetailTimeLineProcessChooseProperty/{familyId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DetailTimeLineProcessChooseProperty([FromRoute] string familyId)
        {

            /*TODO VERIFICAR REGRA SE NECESSARIO AGENDAMENTO DE ESCOLHA DE IMOVEL*/
            try
            {
                var vwScheduleDetailTimeLineChoosePropertyViewModel = new ScheduleDetailTimeLineChoosePropertyViewModel();
                var family = await _familyRepository.FindByIdAsync(familyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                vwScheduleDetailTimeLineChoosePropertyViewModel = _mapper.Map<ScheduleDetailTimeLineChoosePropertyViewModel>(family);

                // var schedule = await _scheduleRepository.FindOneByAsync(x => x.FamilyId == familyId && x.TypeSubject == TypeSubject.EscolhaDoImovel);
                // if (schedule == null)
                //     return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotFound));

                var interest = await _propertiesInterestRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<PropertiesInterest>;
                if (interest.Count() > 0)
                {
                    var residencialPropertyInterest = await _residencialPropertyRepository.FindIn("_id", interest.Select(x => ObjectId.Parse(x.ResidencialPropertyId.ToString())).ToList()) as List<ResidencialProperty>;
                    vwScheduleDetailTimeLineChoosePropertyViewModel.InterestResidencialProperty = _mapper.Map<List<ResidencialPropertyViewModel>>(residencialPropertyInterest);

                    var scheduleHistory = _scheduleHistoryRepository.FindBy(x => x.FamilyId == familyId).OrderBy(x => x.TypeSubject).ThenByDescending(x => x.Created).ToList();
                    vwScheduleDetailTimeLineChoosePropertyViewModel.Schedules = _mapper.Map<List<ScheduleHistoryViewModel>>(scheduleHistory);
                }
                vwScheduleDetailTimeLineChoosePropertyViewModel.InterestResidencialProperty = vwScheduleDetailTimeLineChoosePropertyViewModel.InterestResidencialProperty.OrderByDescending(x => x.Created).ToList();
                vwScheduleDetailTimeLineChoosePropertyViewModel.Schedules = vwScheduleDetailTimeLineChoosePropertyViewModel.Schedules.OrderByDescending(x => x.Created).ToList();

                var listQuizByFamily = await _quizFamilyRepository.FindByAsync(x => x.FamilyId == familyId, Builders<QuizFamily>.Sort.Descending(nameof(QuizFamily.Created))) as List<QuizFamily>;
                var listQuiz = await _quizRepository.FindIn("_id", listQuizByFamily.Select(x => ObjectId.Parse(x.QuizId.ToString())).ToList()) as List<Quiz>;

                for (int i = 0; i < listQuizByFamily.Count(); i++)
                {
                    if (listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).TypeQuiz == TypeQuiz.Quiz)
                    {
                        vwScheduleDetailTimeLineChoosePropertyViewModel.DetailQuiz.Add(new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Created = listQuizByFamily[i].Created,
                            Title = listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString()
                        });
                    }
                    else
                    {
                        vwScheduleDetailTimeLineChoosePropertyViewModel.DetailEnquete.Add(new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Created = listQuizByFamily[i].Created,
                            Title = listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString()
                        });
                    }
                }

                var courseFamily = await _courseFamilyRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<CourseFamily>; ;
                var course = await _courseRepository.FindIn("_id", courseFamily.Select(x => ObjectId.Parse(x.CourseId.ToString())).ToList()) as List<Course>;
                for (int i = 0; i < courseFamily.Count(); i++)
                {
                    vwScheduleDetailTimeLineChoosePropertyViewModel.Courses.Add(new ScheduleCourseViewModel()
                    {
                        Created = courseFamily[i].Created,
                        Title = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).Title,
                        StartDate = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).StartDate,
                        EndDate = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).EndDate,
                        TypeStatusCourse = courseFamily[i].TypeStatusCourse
                    });
                }

                vwScheduleDetailTimeLineChoosePropertyViewModel.Courses = vwScheduleDetailTimeLineChoosePropertyViewModel.Courses.OrderByDescending(x => x.Created).ToList();
                vwScheduleDetailTimeLineChoosePropertyViewModel.DetailQuiz = vwScheduleDetailTimeLineChoosePropertyViewModel.DetailQuiz.OrderByDescending(x => x.Created).ToList();
                vwScheduleDetailTimeLineChoosePropertyViewModel.DetailEnquete = vwScheduleDetailTimeLineChoosePropertyViewModel.DetailEnquete.OrderByDescending(x => x.Created).ToList();

                return Ok(Utilities.ReturnSuccess(data: vwScheduleDetailTimeLineChoosePropertyViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// RETORNA OS  DETALHES DA LINHA DO TEMPO DA FAMÍLIA - ESCOLHA DO IMÓVEL 1 e 2
        /// </summary>
        /// <response code = "200" > Returns success</response>
        /// <response code = "400" > Custom Error</response>
        /// <response code = "401" > Unauthorize Error</response>
        /// <response code = "500" > Exception Error</response>
        /// <returns></returns>
        [HttpGet("DetailTimeLineProcessChoosePropertyOneAndTwo/{familyId}/{typeSubject}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DetailTimeLineProcessChoosePropertyOneAndTwo([FromRoute] string familyId, [FromRoute] TypeSubject typeSubject)
        {
            try
            {
                typeSubject = TypeSubject.Mudanca;
                List<int> enumsValid = new List<int> { (int)TypeSubject.Mudanca, (int)TypeSubject.AcompanhamentoPosMudança };
                if (!enumsValid.Contains((int)typeSubject))
                    return BadRequest(Utilities.ReturnErro("Tipo de Assunto inválido! Deve-se utilizar( Mudança e Acompanhamento pós mudança)"));

                var _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel = new ScheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel();
                var family = await _familyRepository.FindByIdAsync(familyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel = _mapper.Map<ScheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel>(family);

                var scheduleHistory = _scheduleHistoryRepository.FindBy(x => x.FamilyId == familyId && x.TypeSubject == typeSubject).OrderBy(x => x.TypeSubject).ThenBy(x => x.Created).ToList();
                _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel.Schedules = _mapper.Map<List<ScheduleHistoryViewModel>>(scheduleHistory);

                var listQuizByFamily = await _quizFamilyRepository.FindByAsync(x => x.FamilyId == familyId, Builders<QuizFamily>.Sort.Descending(nameof(QuizFamily.Created))) as List<QuizFamily>;
                var listQuiz = await _quizRepository.FindIn("_id", listQuizByFamily.Select(x => ObjectId.Parse(x.QuizId.ToString())).ToList()) as List<Quiz>;

                for (int i = 0; i < listQuizByFamily.Count(); i++)
                {
                    if (listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).TypeQuiz == TypeQuiz.Quiz)
                    {
                        _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel.DetailQuiz.Add(new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Title = listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString()
                        });
                    }
                    else
                    {
                        _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel.DetailEnquete.Add(new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Title = listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString()
                        });
                    }
                }

                var courseFamily = await _courseFamilyRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<CourseFamily>; ;
                var course = await _courseRepository.FindIn("_id", courseFamily.Select(x => ObjectId.Parse(x.CourseId.ToString())).ToList()) as List<Course>;
                for (int i = 0; i < courseFamily.Count(); i++)
                {
                    _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel.Courses.Add(new ScheduleCourseViewModel()
                    {
                        Title = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).Title,
                        StartDate = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).StartDate,
                        EndDate = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).EndDate,
                        TypeStatusCourse = courseFamily[i].TypeStatusCourse
                    });
                }
                return Ok(Utilities.ReturnSuccess(data: _vwcheduleDetailTimeLineProcessChoosePropertyOneAndTwoViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }



        /// <summary>
        /// RETORNA OS  DETALHES DA LINHA DO TEMPO DA FAMÍLIA - REUNIÃO PGM
        /// </summary>
        /// <response code = "200" > Returns success</response>
        /// <response code = "400" > Custom Error</response>
        /// <response code = "401" > Unauthorize Error</response>
        /// <response code = "500" > Exception Error</response>
        /// <returns></returns>
        [HttpGet("DetailTimeLineProcessReunionPGM/{familyId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DetailTimeLineProcessReunionPGM([FromRoute] string familyId)
        {
            try
            {
                var family = await _familyRepository.FindByIdAsync(familyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));
                var listSchedule = await _scheduleRepository.FindByAsync(x => x.FamilyId == familyId && x.TypeSubject == TypeSubject.ReuniaoPGM) as List<Schedule>;
                if (listSchedule.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotFound));

                var listQuizByFamily = await _quizFamilyRepository.FindByAsync(x => x.FamilyId == familyId, Builders<QuizFamily>.Sort.Descending(nameof(QuizFamily.Created))) as List<QuizFamily>;
                var listQuiz = await _quizRepository.FindIn("_id", listQuizByFamily.Select(x => ObjectId.Parse(x.QuizId.ToString())).ToList()) as List<Quiz>;

                var vwTimeLine = _mapper.Map<ScheduleDetailTimeLinePGMViewModel>(listSchedule[0]);

                vwTimeLine.Schedules = _mapper.Map<List<ScheduleViewModel>>(listSchedule);
                for (int i = 0; i < listQuizByFamily.Count(); i++)
                {
                    if (listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).TypeQuiz == TypeQuiz.Quiz)
                    {
                        vwTimeLine.DetailQuiz.Add(new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Title = listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString()
                        });
                    }
                    else
                    {
                        vwTimeLine.DetailEnquete.Add(new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Title = listQuiz.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId)).Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString()
                        });
                    }
                }

                var courseFamily = await _courseFamilyRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<CourseFamily>; ;
                var course = await _courseRepository.FindIn("_id", courseFamily.Select(x => ObjectId.Parse(x.CourseId.ToString())).ToList()) as List<Course>;
                for (int i = 0; i < courseFamily.Count(); i++)
                {
                    vwTimeLine.Courses.Add(new ScheduleCourseViewModel()
                    {
                        Title = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).Title,
                        StartDate = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).StartDate,
                        EndDate = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId)).EndDate,
                        TypeStatusCourse = courseFamily[i].TypeStatusCourse
                    });
                }
                return Ok(Utilities.ReturnSuccess(data: vwTimeLine));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// RETORNA OS DETALHES DA LINHA DO TEMPO DA FAMÍLIA POR TIPO DE AGENDAMENTO
        /// </summary>
        /// <response code = "200" > Returns success</response>
        /// <response code = "400" > Custom Error</response>
        /// <response code = "401" > Unauthorize Error</response>
        /// <response code = "500" > Exception Error</response>
        /// <returns></returns>
        [HttpGet("DetailTimeLine/{familyId}/{typeSubject}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(ScheduleDetailTimeLinePGMViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DetailTimeLine([FromRoute] string familyId, [FromRoute] TypeSubject typeSubject)
        {
            try
            {
                var familyEntity = await _familyRepository.FindByIdAsync(familyId).ConfigureAwait(false);
                if (familyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var listSchedule = await _scheduleRepository.FindByAsync(x => x.FamilyId == familyId) as List<Schedule>;
                if (listSchedule.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotFound));

                var listQuizByFamily = await _quizFamilyRepository.FindByAsync(x => x.FamilyId == familyId, Builders<QuizFamily>.Sort.Descending(nameof(QuizFamily.Created))) as List<QuizFamily>;
                var listQuizEntity = await _quizRepository.FindIn("_id", listQuizByFamily.Select(x => ObjectId.Parse(x.QuizId.ToString())).ToList()) as List<Quiz>;

                var vwTimeLine = _mapper.Map<ScheduleDetailTimeLinePGMViewModel>(listSchedule[0]);

                vwTimeLine.Schedules = _mapper.Map<List<ScheduleViewModel>>(listSchedule.Where(x => x.TypeSubject == typeSubject));

                if (typeSubject == TypeSubject.ReuniaoPGM)
                    vwTimeLine.CanNextStage = listSchedule.Count(x => x.TypeSubject == TypeSubject.EscolhaDoImovel) == 0 && listSchedule.Count(x => x.TypeSubject == TypeSubject.ReuniaoPGM) == listSchedule.Count(x => x.TypeSubject == TypeSubject.ReuniaoPGM && x.TypeScheduleStatus == TypeScheduleStatus.Finalizado);

                if (typeSubject == TypeSubject.ReuniaoPGM || typeSubject == TypeSubject.Mudanca || typeSubject == TypeSubject.AcompanhamentoPosMudança)

                    for (int i = 0; i < listQuizByFamily.Count(); i++)
                    {
                        var quizItem = listQuizEntity.Find(x => x._id == ObjectId.Parse(listQuizByFamily[i].QuizId));

                        var item = new ScheduleQuizDetailTimeLinePGMViewModel()
                        {
                            Title = quizItem.Title,
                            Date = listQuizByFamily[i].Created.Value.TimeStampToDateTime().ToString("dd/MM/yyyy"),
                            HasAnswered = listQuizByFamily[i].TypeStatus == TypeStatus.NaoRespondido ? "Não respondido" : "Respondido",
                            TypeStatus = listQuizByFamily[i].TypeStatus,
                            FamilyId = familyId,
                            QuizId = listQuizByFamily[i].QuizId,
                            QuizFamilyId = listQuizByFamily[i]._id.ToString(),
                            TypeQuiz = listQuizByFamily[i].TypeQuiz,
                            Id = listQuizByFamily[i].QuizId,
                        };

                        if (quizItem.TypeQuiz == TypeQuiz.Quiz)
                            vwTimeLine.DetailQuiz.Add(item);
                        else
                            vwTimeLine.DetailEnquete.Add(item);
                    }


                var courseFamily = await _courseFamilyRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<CourseFamily>; ;
                var course = await _courseRepository.FindIn("_id", courseFamily.Select(x => ObjectId.Parse(x.CourseId.ToString())).ToList()) as List<Course>;
                for (int i = 0; i < courseFamily.Count(); i++)
                {
                    var courseItem = course.Find(x => x._id == ObjectId.Parse(courseFamily[i].CourseId));

                    if (courseItem == null)
                        continue;

                    vwTimeLine.Courses.Add(new ScheduleCourseViewModel()
                    {
                        Id = courseItem._id.ToString(),
                        CourseFamilyId = courseFamily[i]._id.ToString(),
                        Created = courseFamily[i].Created,
                        Title = courseItem.Title,
                        StartDate = courseItem.StartDate,
                        EndDate = courseItem.EndDate,
                        TypeStatusCourse = courseFamily[i].TypeStatusCourse,
                        Schedule = courseItem.Schedule
                    });
                }

                switch (typeSubject)
                {
                    case TypeSubject.EscolhaDoImovel:

                        var familyPropertyInterestList = await _propertiesInterestRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<PropertiesInterest>;

                        if (familyPropertyInterestList.Count() > 0)
                        {
                            var residencialPropertyInterest = await _residencialPropertyRepository.FindIn("_id", familyPropertyInterestList.Select(x => ObjectId.Parse(x.ResidencialPropertyId)).ToList()) as List<ResidencialProperty>;

                            vwTimeLine.InterestResidencialProperty = _mapper.Map<List<ResidencialPropertyViewModel>>(residencialPropertyInterest);
                        }
                        break;
                }

                return Ok(Utilities.ReturnSuccess(data: vwTimeLine));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// MUDA A SITUAÇÃO DA ETAPA - Aguardando confirmação = 0 - Confirmado = 1 - Aguardando reagendamento = 2 - Reagendado = 3 - Finalizado = 4 - Cancelado = 5
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ChangeStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangeStatus([FromBody] ScheduleChangeStatusViewModel model)
        {

            try
            {

                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Date), nameof(model.Description), nameof(model.FamilyId), nameof(model.Place));
                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var scheduleEntity = await _scheduleRepository.FindByIdAsync(model.Id).ConfigureAwait(false);

                if (model.TypeScheduleStatus != TypeScheduleStatus.Cancelado && model.TypeScheduleStatus
                    != TypeScheduleStatus.Finalizado && model.TypeScheduleStatus != TypeScheduleStatus.Confirmado && model.TypeScheduleStatus != TypeScheduleStatus.Reagendado)
                {
                    var dateToSchedule = Utilities.TimeStampToDateTime(model.Date);
                    if (dateToSchedule < DateTime.Now)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.DateInvalidToSchedule));

                }

                var family = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var oldDate = Utilities.TimeStampToDateTime(scheduleEntity.Date).ToString("dd/MM/yyyy HH:mm");

                scheduleEntity.TypeScheduleStatus = model.TypeScheduleStatus;
                scheduleEntity.Description = model.Description;
                scheduleEntity.Place = model.Place;

                if (model.TypeScheduleStatus != TypeScheduleStatus.Finalizado)
                    scheduleEntity.Date = model.Date;

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                if (model.TypeScheduleStatus == TypeScheduleStatus.Finalizado)
                {
                    scheduleEntity.DateFinished = now;
                }

                if (model.TypeScheduleStatus == TypeScheduleStatus.AguardandoReagendamento || model.TypeScheduleStatus == TypeScheduleStatus.Cancelado || model.TypeScheduleStatus == TypeScheduleStatus.Confirmado || model.TypeScheduleStatus == TypeScheduleStatus.Reagendado)
                {
                    await _scheduleRepository.UpdateAsync(scheduleEntity).ConfigureAwait(false);
                }

                var scheduleHistoryList = await _scheduleHistoryRepository.FindByAsync(x => x.TypeSubject == scheduleEntity.TypeSubject && x.FamilyId == model.FamilyId).ConfigureAwait(false) as List<ScheduleHistory>;
                var scheduleHistoryEntity = _mapper.Map<ScheduleHistory>(model);
                scheduleHistoryEntity.HolderCpf = family.Holder.Cpf;
                scheduleHistoryEntity.HolderName = family.Holder.Name;
                scheduleHistoryEntity.HolderNumber = family.Holder.Number;
                scheduleHistoryEntity.TypeSubject = scheduleEntity.TypeSubject;
                scheduleHistoryEntity.TypeScheduleStatus = model.TypeScheduleStatus;
                scheduleHistoryEntity.ScheduleId = scheduleEntity._id.ToString();
                scheduleHistoryEntity.ParentId = scheduleHistoryList?.LastOrDefault()?._id.ToString();
                scheduleHistoryEntity._id = new ObjectId();
                await _scheduleHistoryRepository.CreateAsync(scheduleHistoryEntity).ConfigureAwait(false);


                bool sendNotify = false;
                string title = null;
                string secondTitle = null;
                string content = null;
                string secondContent = null;
                ForType forType = ForType.Family;
                // TODO: pending continue in second phase with the finality of show notifications for another roles
                //ForType secondForType = model.Profile != null ? (ForType) model.Profile : ForType.TTS;
                //ForType replyForType = model.Profile != null && (ForType) model.Profile == ForType.TTS ? ForType.Administrador : ForType.TTS;
                
                bool multipleNotify = false;

                var message = new StringBuilder();
                var secondMessage = new StringBuilder();

                switch (model.TypeScheduleStatus)
                {
                    case TypeScheduleStatus.AguardandoConfirmacao:
                        sendNotify = true;
                        title = "Pedido de confirmação do agendamento";

                        message.AppendLine($"<p>Olá {family.Holder.Name},<br/>");
                        message.AppendLine($"Solicitamos agendamento da família {family.Holder.Number}, queremos que confirmem o agendamento</p>");

                        // TODO: pending check if in a second phase need remove o conserve
                        //if (secondForType == ForType.TTS)
                        //{
                        //    multipleNotify = true;
                        //    secondTitle = $"SPAM: O perfil {secondForType.ToString()} mediante solicitação confirmação de agendamento para o proprietário {family.Holder.Name}";

                        //    secondMessage.AppendLine($"<p>Número do titular: {family.Holder.Number}<br/>");
                        //    secondMessage.AppendLine($"</p>");
                        //    secondForType = replyForType;
                        //}

                        break;
                    case TypeScheduleStatus.Confirmado:
                        sendNotify = true;
                        forType = ForType.TTS;
                        title = "Um agendamento foi confirmado";

                        message.AppendLine("<p>Olá equipe TTS<br/>");
                        message.AppendLine($"Um agendamento para família {family.Holder.Number} foi confirmado</p>");
                        content = message.ToString();

                        if (sendNotify)
                        {
                            await _utilService.SendNotify(title, content, null, family.DeviceId, forType, family._id.ToString(), null, null,
                                UtilService.getModuleSchedule(model.TypeScheduleStatus, scheduleEntity._id.ToString()));

                        }

                        var entityProfile = await _profileRepository.FindByAsync(x => x.TypeProfile == TypeUserProfile.Gestor || x.TypeProfile == TypeUserProfile.TTS);
                        foreach (var item in entityProfile)
                        {
                            var dataBody = Util.GetTemplateVariables();
                            dataBody.Add("{{ title }}", title);
                            dataBody.Add("{{ message }}", content);

                            var body = _senderMailService.GerateBody("custom", dataBody);

                            var unused = Task.Run(async () =>
                            {
                                await _senderMailService.SendMessageEmailAsync("Moralar", item.Email, body, title).ConfigureAwait(false);
                            });

                        }

                            break;
                    case TypeScheduleStatus.AguardandoReagendamento:
                        sendNotify = true;
                        title = "Aviso de reagendamento";

                        message.AppendLine($"<p>Olá {family.Holder.Name},<br/>");
                        message.AppendLine($"Seu agendamento do dia {oldDate} foi reagendado para a data: {Utilities.TimeStampToDateTimeLocalZone(model.Date):dd/MM/yyyy HH:mm}  e precisa da sua confirmação.</p>");

                        break;
                    case TypeScheduleStatus.Reagendado:
                        sendNotify = true;
                        forType = ForType.TTS;
                        title = "Confirmação de reagendamento";

                        message.AppendLine("<p>Olá equipe TTS<br/>");
                        message.AppendLine($"a família do número {family.Holder.Number} confirmou o reagendamento</p>");
                        content = message.ToString();

                        if (sendNotify)
                        {
                            await _utilService.SendNotify(title, content, null, family.DeviceId, forType, family._id.ToString(), null, null,
                                UtilService.getModuleSchedule(model.TypeScheduleStatus, scheduleEntity._id.ToString()));

                        }

                        var entityProfile2 = await _profileRepository.FindByAsync(x => x.TypeProfile == TypeUserProfile.Gestor || x.TypeProfile == TypeUserProfile.TTS);
                        foreach (var item in entityProfile2)
                        {
                            var dataBody = Util.GetTemplateVariables();
                            dataBody.Add("{{ title }}", title);
                            dataBody.Add("{{ message }}", content);

                            var body = _senderMailService.GerateBody("custom", dataBody);

                            var unused = Task.Run(async () =>
                            {
                                await _senderMailService.SendMessageEmailAsync("Moralar", item.Email, body, title).ConfigureAwait(false);
                            });

                        }

                        break;
                    case TypeScheduleStatus.Cancelado:
                        sendNotify = true;
                        title = "Aviso de cancelamento de agendamento";

                        message.AppendLine($"<p>Olá {family.Holder.Name},<br/>");
                        message.AppendLine($"Seu agendamento do dia {oldDate} Foi cancelado por um de nossos consultores, caso tenha alguma dúvida entre em contato conosco.</p>");

                        // TODO: pending in second phase notify if another person need notification of that a tts make a cancel

                        break;
                    case TypeScheduleStatus.Finalizado:
                        sendNotify = true;
                        title = "Processo de agendamento concluído";

                        message.AppendLine($"<p>Olá {family.Holder.Name},<br/>");
                        message.AppendLine($"Informamos que o seu processo de agendamento foi concluído corretamente, agradecemos a sua participação.</p>");
                        break;
                }

                content = message.ToString();
                secondContent = secondMessage.ToString();

                if (sendNotify && forType == ForType.Family)
                {
                    await _utilService.SendNotify(title, content, family.Holder.Email, family.DeviceId, forType, family._id.ToString(), null, null,
                        UtilService.getModuleSchedule(model.TypeScheduleStatus, scheduleEntity._id.ToString()));

                }

                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// MUDA A SITUAÇÃO DA ETAPA DOS PROCESSOS DE REASSENTAMENTO  - ReuniaoPGM = 2 - EscolhaDoImovel = 4 - Mudanca = 7 - AcompanhamentoPosMudança = 8
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ChangeSubject")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangeSubject([FromBody] ScheduleChangeSubjectViewModel model)
        {

            try
            {
                //Reunião PGM -  ReuniaoPGM = 2,
                //Escolha do imóvel -  EscolhaDoImovel = 7,
                //Mudança -    Mudanca = 4,
                //Acompanhamento pós-mudança -  AcompanhamentoPosMudança = 8
                List<int> enumsValid = new List<int> { (int)TypeSubject.ReuniaoPGM, (int)TypeSubject.EscolhaDoImovel, (int)TypeSubject.Mudanca, (int)TypeSubject.AcompanhamentoPosMudança };
                if (!enumsValid.Contains((int)model.TypeSubject))
                    return BadRequest(Utilities.ReturnErro("Tipo de Assunto inválido! Deve-se utilizar(Reunião Pgm - Escolha do Imóvel - Mudança - Acompanhamento pós mudança)"));


                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Date), nameof(model.Description), nameof(model.FamilyId), nameof(model.Place));
                if (isInvalidState != null)
                    return BadRequest(isInvalidState);


                var dateToSchedule = Utilities.TimeStampToDateTime(model.Date);
                if (dateToSchedule < DateTime.Today)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.DateInvalidToSchedule));

                var family = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));


                var scheduleEntity = await _scheduleRepository.FindByIdAsync(model.Id).ConfigureAwait(false);

                if (scheduleEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotFound));

                if (scheduleEntity.TypeScheduleStatus != TypeScheduleStatus.Finalizado)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ChangeSubject));

                scheduleEntity.Date = model.Date;
                scheduleEntity.Description = model.Description;
                scheduleEntity.Place = model.Place;
                scheduleEntity.TypeSubject = model.TypeSubject;
                scheduleEntity.TypeScheduleStatus = TypeScheduleStatus.AguardandoConfirmacao;
                await _scheduleRepository.UpdateAsync(scheduleEntity).ConfigureAwait(false);


                var scheduleHistoryList = await _scheduleHistoryRepository.FindByAsync(x => x.TypeSubject == scheduleEntity.TypeSubject && x.FamilyId == model.FamilyId).ConfigureAwait(false) as List<ScheduleHistory>;
                if (scheduleHistoryList.Exists(x => x.TypeSubject == TypeSubject.EscolhaDoImovel || x.TypeSubject == TypeSubject.Mudanca || x.TypeSubject == TypeSubject.AcompanhamentoPosMudança))
                {
                    //var c = scheduleHistoryList.Where(x => x.TypeSubject == scheduleEntity.TypeSubject);
                    await _scheduleHistoryRepository.UpdateOneAsync(scheduleHistoryList.FirstOrDefault()).ConfigureAwait(false);
                }
                else
                {
                    model.Id = null;
                    var scheduleHistoryEntity = _mapper.Map<ScheduleHistory>(model);
                    scheduleHistoryEntity.HolderCpf = family.Holder.Cpf;
                    scheduleHistoryEntity.HolderName = family.Holder.Name;
                    scheduleHistoryEntity.HolderNumber = family.Holder.Number;
                    scheduleHistoryEntity.TypeScheduleStatus = scheduleEntity.TypeScheduleStatus;
                    scheduleHistoryEntity.ScheduleId = scheduleEntity._id.ToString();
                    await _scheduleHistoryRepository.CreateAsync(scheduleHistoryEntity).ConfigureAwait(false);
                }
                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
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


        public async Task<IActionResult> Export([FromForm] DtParameters model, [FromForm] string number, [FromForm] string name, [FromForm] string cpf, [FromForm] long? startDate, [FromForm] long? endDate, [FromForm] string place, [FromForm] string description, [FromForm] TypeScheduleStatus? status, [FromForm] TypeSubject? type)
        {
            var response = new DtResult<ScheduleExportViewModel>();
            try
            {
                var builder = Builders<Data.Entities.Schedule>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.Schedule>>();

                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null));

                if (!string.IsNullOrEmpty(number))
                    conditions.Add(builder.Where(x => x.HolderNumber == number));
                if (!string.IsNullOrEmpty(name))
                    conditions.Add(builder.Where(x => x.HolderName.ToUpper().Contains(name.ToUpper())));
                if (!string.IsNullOrEmpty(cpf))
                    conditions.Add(builder.Where(x => x.HolderCpf == cpf.OnlyNumbers()));
                if (startDate.HasValue)
                    conditions.Add(builder.Where(x => x.Date >= startDate));
                if (endDate.HasValue)
                    conditions.Add(builder.Where(x => x.Date <= endDate));
                if (!string.IsNullOrEmpty(place))
                    conditions.Add(builder.Where(x => x.Place.ToUpper().Contains(place.ToUpper())));
                if (!string.IsNullOrEmpty(description))
                    conditions.Add(builder.Where(x => x.Description.ToUpper().Contains(description.ToUpper())));

                if (status != null)
                    conditions.Add(builder.Where(x => x.TypeScheduleStatus == status));

                if (type != null)
                    conditions.Add(builder.Where(x => x.TypeSubject == type));


                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _scheduleRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.Schedule>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.Schedule>.Sort.Ascending(sortColumn);

                var allData = await _scheduleRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, totalRecords, conditions, columns);

                var condition = builder.And(conditions);
                var fileName = "Agendamentos_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";

                var path = UtilService.getFilePathFromServer("ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);
                var listViewModel = _mapper.Map<List<ScheduleExportViewModel>>(allData);
                Utilities.ExportToExcel(listViewModel, path, fileName: fileName.Split('.')[0]);
                if (System.IO.File.Exists(fullPathFile) == false)
                    return BadRequest(Utilities.ReturnErro("Ocorreu um erro fazer download do arquivo"));

                var fileBytes = System.IO.File.ReadAllBytes(@fullPathFile);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(Utilities.ReturnErro(ex.Message));
            }
        }

        /// <summary>
        /// EXPORTAR DADOS MAPA DE DESLOCAMENTO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ExportMap")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportMap([FromForm] DtParameters model, [FromForm] string number)
        {

            try
            {
                var builder = Builders<Schedule>.Filter;
                var conditions = new List<FilterDefinition<Schedule>>();

                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null && x.TypeSubject == TypeSubject.Mudanca));

                if (!string.IsNullOrEmpty(number))
                    conditions.Add(builder.Where(x => x.HolderNumber == number));

                var columns = model.Columns.Where(x => x.Searchable && string.IsNullOrEmpty(x.Name) == false).Select(x => x.Name).ToArray();

                var sortColumn = model.SortOrder;
                var totalRecords = (int)await _scheduleRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                   ? Builders<Schedule>.Sort.Descending(sortColumn)
                   : Builders<Schedule>.Sort.Ascending(sortColumn);

                var retorno = await _scheduleRepository
                 .LoadDataTableAsync(model.Search.Value, sortBy, 0, 0, conditions, columns) as List<Schedule>;


                var response = _mapper.Map<List<ScheduleMapExportViewModel>>(retorno);


                for (int i = 0; i < retorno.Count(); i++)
                {
                    var item = retorno[i];

                    var family = await _familyRepository.FindByIdAsync(item.FamilyId).ConfigureAwait(false);

                    var infoOrigin = await _utilService.GetInfoFromZipCode(family.Address.CEP).ConfigureAwait(false);
                    if (infoOrigin == null)
                        return BadRequest(Utilities.ReturnErro("Cep não encontrado"));

                    var residencialOrigin = Utilities.GetInfoFromAdressLocation(infoOrigin.StreetAddress + ", " + infoOrigin.Number + " " + infoOrigin.Complement + " " + infoOrigin.Neighborhood + " " + infoOrigin.CityName + " " + infoOrigin.StateUf);
                    if (residencialOrigin.Erro == true)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.LocationNotFound));


                    var propertyInterest = await _propertiesInterestRepository.FindByAsync(x => x.FamilyId == item.FamilyId).ConfigureAwait(false);

                    if (propertyInterest == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.PropertyNotChosen));

                    var residencialDestination = await _residencialPropertyRepository.FindIn(c => c.TypeStatusResidencialProperty == TypeStatusResidencial.Vendido, "_id", propertyInterest.Select(x => ObjectId.Parse(x.ResidencialPropertyId.ToString())).ToList(), Builders<ResidencialProperty>.Sort.Ascending(nameof(ResidencialProperty.LastUpdate))) as List<ResidencialProperty>;

                    if (residencialDestination.FirstOrDefault() == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.PropertySaledNotFound));

                    if (residencialDestination.Count() > 1)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialSaled));

                    var infoDestination = await _utilService.GetInfoFromZipCode(residencialDestination.FirstOrDefault().ResidencialPropertyAdress.CEP).ConfigureAwait(false);
                    if (infoDestination == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.PropertySaledNotFound));

                    var destination = Utilities.GetInfoFromAdressLocation(infoDestination.StreetAddress + ", " + infoDestination.Number + " " + infoDestination.Complement + " " + infoDestination.Neighborhood + " " + infoDestination.CityName + " " + infoDestination.StateUf);
                    var distance = Utilities.GetDistance(residencialOrigin.Geometry.Location.Lat, residencialOrigin.Geometry.Location.Lng, destination.Geometry.Location.Lat, destination.Geometry.Location.Lng);

                    response[i].AddressPropertyDistanceMeters = (distance / 1000).ToString("0.###");
                    response[i].AddressPropertyDistanceKilometers = distance.ToString("0.##");
                    response[i].AddressPropertyOrigin = residencialOrigin.FormatedAddress;
                    response[i].AddressPropertyDestination = destination.FormatedAddress;

                }


                var fileName = "Mapa de deslocamento.xlsx";
                var path = UtilService.getFilePathFromServer("/Content/ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);

                Utilities.ExportToExcel(response, path, "Mapa", fileName.Split('.')[0]);
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