using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Question;
using Moralar.Domain.ViewModels.QuestionAnswer;
using Moralar.Domain.ViewModels.Quiz;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;
using Org.BouncyCastle.Utilities;
using System.Text.RegularExpressions;


namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class QuizController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionDescriptionRepository _questionDescriptionRepository;
        private readonly IQuestionAnswerRepository _questionAnswerRepository;
        private readonly IQuizFamilyRepository _quizFamilyRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUtilService _utilService;
        private readonly ISenderNotificationService _senderNotificationService;

        public QuizController(IMapper mapper, IQuestionRepository questionRepository, IQuizRepository quizRepository, IQuestionDescriptionRepository questionDescriptionRepository, IQuizFamilyRepository quizFamilyRepository, IFamilyRepository familyRepository, INotificationRepository notificationRepository, IUtilService utilService, ISenderNotificationService senderNotificationService, IQuestionAnswerRepository questionAnswerRepository)
        {
            _mapper = mapper;
            _questionRepository = questionRepository;
            _quizRepository = quizRepository;
            _questionDescriptionRepository = questionDescriptionRepository;
            _quizFamilyRepository = quizFamilyRepository;
            _familyRepository = familyRepository;
            _notificationRepository = notificationRepository;
            _utilService = utilService;
            _senderNotificationService = senderNotificationService;
            _questionAnswerRepository = questionAnswerRepository;
        }

        /// <summary>
        /// BLOQUEAR / DESBLOQUEAR QUIZ
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "targetId": "string", // required
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

                var entity = await _quizRepository.FindByIdAsync(model.TargetId);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));
                entity.DataBlocked = model.Block ? DateTimeOffset.Now.ToUnixTimeSeconds() : (long?)null;

                await _quizRepository.UpdateAsync(entity);
                var typeAction = model.Block == true ? TypeAction.Block : TypeAction.UnBlock;
                await _utilService.RegisterLogAction(LocalAction.Familia, typeAction, TypeResposible.UserAdminstratorGestor, $"Bloqueio de família {Request.GetUserName()}", Request.GetUserId(), Request.GetUserName()?.Value, model.TargetId);
                return Ok(Utilities.ReturnSuccess(model.Block ? "Bloqueado com sucesso" : "Desbloqueado com sucesso"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Familia, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível bloquer Família", Request.GetUserId(), Request.GetUserName()?.Value, model.TargetId, "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// LISTA OS QUESTIONÁRIOS/ENQUETES POR NOME E TIPO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetByName")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(QuizListViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByName(string nameQuiz, TypeQuiz typeQuiz, [FromQuery] bool? active)
        {
            try
            {
                var userId = Request.GetUserId();

                var conditions = new List<FilterDefinition<Data.Entities.Quiz>>();
                var builder = Builders<Data.Entities.Quiz>.Filter;
                conditions.Add(builder.Where(x => x.Created != null && x._id != null));

                if (string.IsNullOrEmpty(nameQuiz) == false)
                    conditions.Add(builder.Regex(x => x.Title, new BsonRegularExpression(new Regex(nameQuiz, RegexOptions.IgnoreCase))));

                conditions.Add(builder.Where(x => x.TypeQuiz == typeQuiz));

                if (active != null && (bool) active)
                {
                    conditions.Add(builder.Where(x => x.DataBlocked == null));
                }

                var listQuiz = await _quizRepository.GetCollectionAsync().FindSync(builder.And(conditions), new FindOptions<Data.Entities.Quiz>() { }).ToListAsync();
                if (listQuiz.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));

                var listQuizFamilies = await _quizFamilyRepository.FindByAsync(x => x.FamilyId == userId) as List<QuizFamily>;

                var response = _mapper.Map<List<Quiz>, List<QuizListViewModel>>(listQuiz, opt => opt.AfterMap((src, dest) =>
                {

                    for (int i = 0; i < src.Count(); i++)
                    {
                        var quizfamilyEntity = listQuizFamilies.Find(x => x.QuizId == src[i]._id.ToString());
                        if (quizfamilyEntity != null)
                        {
                            dest[i].TypeStatus = quizfamilyEntity.TypeStatus;
                            dest[i].FamilyId = userId;
                        }
                    }
                }));

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<QuizListViewModel>>(listQuiz)));
            }
            catch (Exception ex)
            {
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
        public async Task<IActionResult> Export([FromForm] DtParameters model, [FromForm] string title, [FromForm] TypeQuiz typeQuiz, [FromForm] List<string> id)
        {
            try
            {

                var builder = Builders<Quiz>.Filter;
                var conditions = new List<FilterDefinition<Quiz>>();

                conditions.Add(builder.Where(x => x.Disabled == null));


                if (string.IsNullOrEmpty(title) == false)
                    conditions.Add(builder.Regex(x => x.Title, new BsonRegularExpression(title, "i")));

                conditions.Add(builder.Where(x => x.TypeQuiz == typeQuiz));

                List<QuizFamily>? quizFamilies = [];

                if (id.Count > 0)
                {

                    List<ObjectId> objectIds = id.Select(listId => new ObjectId(listId)).ToList();

                    quizFamilies = await _quizFamilyRepository.FindIn(nameof(QuizFamily._id), objectIds.Select(x => x).ToList()) as List<QuizFamily>;

                    if (quizFamilies.Count > 0)
                    {
                        List<ObjectId> quizesId = quizFamilies.Select(listId => new ObjectId(listId.QuizId)).ToList();

                        conditions.Add(builder.In(x => x._id, quizesId));
                    }

                }

                var columns = model.Columns.Where(x => x.Searchable && string.IsNullOrEmpty(x.Name) == false).Select(x => x.Name).ToArray();

                var sortColumn = model.SortOrder;
                var totalRecords = (int)await _quizRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                   ? Builders<Quiz>.Sort.Descending(sortColumn)
                   : Builders<Quiz>.Sort.Ascending(sortColumn);

                var retorno = await _quizRepository
                 .LoadDataTableAsync(model.Search.Value, sortBy, 0, 0, conditions, columns) as List<Quiz>;

                var listAnswer = await _questionAnswerRepository.FindIn(nameof(QuestionAnswer.QuizId), retorno.Select(x => x._id.ToString()).ToList()) as List<QuestionAnswer>;
                
                var response = _mapper.Map<List<Quiz>, List<QuizExportViewModel>>(retorno);


                for (int i = 0; i < retorno.Count; i++)
                {
                    var quizId = retorno[i]._id.ToString();

                    response[i].Id = quizId;
                    response[i].TotalAnswers = listAnswer.Where(x => x.QuizId == quizId)
                        // old version
                        //.DistinctBy(x => x.FamilyHolderCpf).Count();
                        // TODO: pending check: need test if these 3 lines make the same like a old versión commented
                        .GroupBy(x => x.FamilyHolderCpf)
                        .Select(g => g.First())
                        .Count();
                }

                var listQuestion = await _questionRepository.FindIn("_id", listAnswer.SelectMany(x => x.Questions).Select(x => ObjectId.Parse(x.QuestionId)).Distinct().ToList()) as List<Question>;

                var answersResponse = new List<QuestionAnswerExportViewModel>();

                for (int i = 0; i < listQuestion.Count(); i++)
                {
                    var questionItem = listQuestion[i];
                    var quizItem = retorno.Find(x => x._id.ToString() == questionItem.QuizId);
                    var questionId = questionItem._id.ToString();
                    var listAnswerItens = listAnswer.Where(x => x.Questions.Count(y => y.QuestionId == questionId) > 0).ToList();

                    var holders = listAnswerItens.GroupBy(x => x.FamilyHolderCpf.OnlyNumbers()).Select(x => x.FirstOrDefault()).ToList();
                    for (int a = 0; a < holders.Count(); a++)
                    {
                        var holderItem = holders[a];

                        var holderAnswer = listAnswerItens.Where(x => x.Questions.Count(y => y.QuestionId == questionId) > 0 && x.FamilyHolderCpf == holderItem.FamilyHolderCpf).SelectMany(x => x.Questions).ToList();

                        answersResponse.Add(new QuestionAnswerExportViewModel()
                        {
                            QuizId = questionItem.QuizId,
                            QuizTitle = quizItem?.Title,
                            QuestionId = questionId,
                            QuestionName = questionItem.NameQuestion,
                            HolderCpf = holderItem.FamilyHolderCpf,
                            HolderName = holderItem.FamilyHolderName,
                            TypeQuestion = questionItem.TypeResponse.GetEnumMemberValue(),
                            Answers = Util.MapAnswer(questionId, holderAnswer)
                        });
                    }
                }

                var fileName = "Questionario_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
                var path = UtilService.getFilePathFromServer("/Content/ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);

                Utilities.ExportToExcel(response, path, "Questionario", fileName.Split('.')[0]);

                /*WORKSHEET COM RESPOSTAS*/
                Util.ExportToExcel(answersResponse, path, "Respostas", fileName.Split('.')[0], addWorksheet: true);

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



        /// <summary>
        /// LISTAGEM DOS QUESTINÁRIOS DATATABLE
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(QuizViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] string title, [FromForm] TypeQuiz typeQuiz)
        {
            var response = new DtResult<QuizViewModel>();
            try
            {
                var builder = Builders<Data.Entities.Quiz>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.Quiz>>();

                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null));

                if (!string.IsNullOrEmpty(title))
                    conditions.Add(builder.Where(x => x.Title == title));

                conditions.Add(builder.Where(x => x.TypeQuiz == typeQuiz));
                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _quizRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.Quiz>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.Quiz>.Sort.Ascending(sortColumn);

                var retorno = await _quizRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                    ? (int)await _quizRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                    : totalRecords;

                response.Data = _mapper.Map<List<QuizViewModel>>(retorno);
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
                var entity = await _quizRepository.FindByIdAsync(id).ConfigureAwait(false);
                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));

                var question = await _questionRepository.FindByAsync(x => x.QuizId == entity._id.ToString() && x.Disabled == null).ConfigureAwait(false) as List<Question>;
                if (question.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuestionNotFound));

                var _quizViewModel = new QuizDetailViewModel()
                {
                    Id = entity._id.ToString(),
                    Title = entity.Title,
                    TypeQuiz = entity.TypeQuiz,
                    Created = entity.Created
                };

                var questionDescription = await _questionDescriptionRepository.FindIn("QuestionId", question.Select(x => ObjectId.Parse(x._id.ToString())).ToList()) as List<QuestionDescription>;
                for (int i = 0; i < question.Count(); i++)
                {
                    _quizViewModel.QuestionViewModel.Add(_mapper.Map<QuestionViewModel>(question[i]));
                    foreach (var item in questionDescription.Where(x => x.QuestionId == question[i]._id.ToString() && x.Disabled == null))
                    {
                        if (item != null)
                            _quizViewModel.QuestionViewModel[i].Description.Add(new QuestionDescriptionViewModel()
                            {
                                Id = item._id.ToString(),
                                Description = item.Description,
                                QuestionId = item.QuestionId
                            });
                    }
                }
                return Ok(Utilities.ReturnSuccess(data: _quizViewModel));
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
        ///          "title": "string",
        ///           "typeQuiz": 0 = Questionário, 1 = Enquete,
        ///          "questionRegister": {
        ///            "question": [
        ///              {
        ///                "nameQuestion": "string",
        ///                "typeResponse": "Texto", Enum Texto = 0,MultiplaEscolha = 1,EscolhaUnica = 2,ListaSuspensa = 3
        ///                "description": [
        ///                  {
        ///                    "description": "string"
        ///                  }
        ///                ]
        ///              }
        ///            ]
        ///          }
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
        public async Task<IActionResult> Register([FromBody] QuizViewModel model)
        {

            try
            {
                //var typeRegister = string.IsNullOrEmpty(model.Id) ? TypeAction.Register : TypeAction.Change;
                //var message = (typeRegister == TypeAction.Register) ? $"Cadastro de novo Questinário {Request.GetUserName()}" : $"Atualização do Questinário {Request.GetUserName()}";
                var ignoreValidation = new List<string>();
                var isInvalidState = ModelState.ValidModelState(ignoreValidation.ToArray());
                if (isInvalidState != null)
                    return BadRequest(isInvalidState);
                var families = await _familyRepository.FindByAsync(x => x.DataBlocked == null).ConfigureAwait(false) as List<Family>;
                var quizEntity = _mapper.Map<Quiz>(model);
                var quizEntityId = await _quizRepository.CreateAsync(quizEntity).ConfigureAwait(false);

                List<string> itensAdded = new List<string>();
                foreach (var item in model.QuestionRegister.Question)
                {
                    var entity = _mapper.Map<Question>(item);
                    entity.QuizId = quizEntityId;
                    var itemAdded = await _questionRepository.CreateAsync(entity).ConfigureAwait(false);

                    foreach (var itemDescription in item.Description)
                    {
                        var description = new QuestionDescription()
                        {
                            Description = itemDescription.Description,
                            QuestionId = itemAdded
                        };
                        await _questionDescriptionRepository.CreateAsync(description).ConfigureAwait(false);
                    }
                    itensAdded.Add(itemAdded);
                }
                if (model.TypeQuiz == TypeQuiz.Enquete)
                {
                    foreach (var item in families)
                    {
                        _quizFamilyRepository.Create(new QuizFamily()
                        {
                            FamilyId = item._id.ToString(),
                            QuizId = quizEntityId,
                            HolderName = item.Holder.Name,
                            HolderCpf = item.Holder.Cpf,
                            HolderNumber = item.Holder.Number,
                            TypeStatus = TypeStatus.NaoRespondido,
                            TypeQuiz = quizEntity.TypeQuiz,
                            Title = quizEntity.Title
                        });
                    }


                    //// var families = new List<Family>();

                    // entityFamily = await _familyRepository.FindByAsync(x => x.Disabled == null).ConfigureAwait(false) as List<Family>;

                    var title = "Nova enquete";
                    var content = "Uma nova enquete foi disponibilizada";

                    var listNotification = new List<Notification>();
                    for (int i = 0; i < families.Count(); i++)
                    {
                        var familyItem = families[i];

                        listNotification.Add(new Notification()
                        {
                            For = ForType.Family,
                            FamilyId = familyItem._id.ToString(),
                            Title = title,
                            Description = content
                        });
                    }

                    await _notificationRepository.CreateAsync(listNotification);

                    dynamic payloadPush = Util.GetPayloadPush();
                    dynamic settingPush = Util.GetSettingsPush();

                    await _senderNotificationService.SendPushAsync(title, content, families.SelectMany(x => x.DeviceId).ToList(), data: payloadPush, settings: settingPush, priority: 10);

                }
                //await _utilService.RegisterLogAction(LocalAction.Question, typeRegister, TypeResposible.UserAdminstratorGestor, message, Request.GetUserId(), Request.GetUserName()?.Value, string.Join(";", itensAdded.ToArray()), "");

                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// APLICA UM QUESTIONÁRIO A UMA DETERMINADA FAMÍLIA OU A TODAS FAMÍLIA
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///
        /// 
        ///         POST - TODAS FAMÍLIAS
        ///         {
        ///          "quizId": "string",
        ///          "allFamily": true // SE DEIXAR TRUE VAI SER ADICIONÁRIO A TODAS FAMÍLIAS
        ///         }
        ///         POST - IDENTIFICANDO AS FAMÍLIAS
        ///         {
        ///          "quizId": "string",
        ///          "allFamily": false, // PRECISA IDENTIFICAR AS FAMÍLIAS ESCOLHIDAS PARA RESPONDER O QUESTIONÁRIO
        ///          "familyId": [
        ///            "string"
        ///          ]
        ///         }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("RegisterQuizToFamily")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterQuizToFamily([FromBody] QuizFamilyViewModel model)
        {

            try
            {
                var quizEntity = await _quizRepository.FindByIdAsync(model.QuizId).ConfigureAwait(false);
                if (quizEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));



                var listFamilyEntity = new List<Family>();

                if (model.AllFamily == true)
                    listFamilyEntity = await _familyRepository.FindByAsync(x => x.Disabled == null).ConfigureAwait(false) as List<Family>;
                else
                {
                    listFamilyEntity = await _familyRepository.FindIn("_id", model.FamilyId.Select(x => ObjectId.Parse(x)).ToList()) as List<Family>;
                    if (model.FamilyId.Count() != listFamilyEntity.Count())
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));
                }

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var alreadyRegistred = await _quizFamilyRepository.FindByAsync(x => x.QuizId == model.Id);

                listFamilyEntity = listFamilyEntity.Where(x => alreadyRegistred.Count(y => y.FamilyId == x._id.ToString()) == 0).ToList();

                foreach (var familyEntity in listFamilyEntity)
                {
                    var entityQuizFamily = new QuizFamily()
                    {
                        Created = now,
                        FamilyId = familyEntity._id.ToString(),
                        QuizId = model.QuizId,
                        HolderName = familyEntity.Holder.Name,
                        HolderCpf = familyEntity.Holder.Cpf,
                        HolderNumber = familyEntity.Holder.Number,
                        TypeStatus = TypeStatus.NaoRespondido,
                        TypeQuiz = quizEntity.TypeQuiz,
                        Title = quizEntity.Title
                    };

                    await _quizFamilyRepository.CreateAsync(entityQuizFamily).ConfigureAwait(false);


                    var notificationEntity = new Notification
                    {
                        Title = "Novo questionário disponibilizado",
                        Description = $"Olá {familyEntity.Holder.Name}, Você tem um novo questionário disponível!",
                        FamilyId = familyEntity._id.ToString(),
                    };

                    /// ENVIAR MSG PARA O APP DO CLIENTE
                    await _notificationRepository.CreateAsync(notificationEntity);

                    dynamic payloadPush = Util.GetPayloadPush(RouteNotification.System);
                    dynamic settingsPush = Util.GetSettingsPush();

                    await _senderNotificationService.SendPushAsync(notificationEntity.Title, notificationEntity.Description, familyEntity.DeviceId, data: payloadPush, settings: settingsPush, priority: 10);

                }

                //await _utilService.RegisterLogAction(LocalAction.Question, typeRegister, TypeResposible.UserAdminstratorGestor, message, Request.GetUserId(), Request.GetUserName()?.Value, string.Join(";", itensAdded.ToArray()), "");



                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTA OS QUESTIONÁRIOS DISPONÍVEIS PARA FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetQuizByFamily")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(QuizViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQuizByFamily(TypeQuiz typeQuiz)
        {
            var _quizViewModel = new List<QuizViewModel>();
            try
            {
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                var quizFamilies = await _quizFamilyRepository.FindByAsync(x => x.DataBlocked == null && x.FamilyId == userId).ConfigureAwait(false) as List<QuizFamily>;

                if (quizFamilies.Count() == 0 && typeQuiz == TypeQuiz.Enquete)
                {
                    var familyEntity = await _familyRepository.FindByIdAsync(userId);

                    if (familyEntity == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                    var listQuiz = await _quizRepository.FindByAsync(x => x.TypeQuiz == TypeQuiz.Enquete) as List<Quiz>;

                    for (int i = 0; i < listQuiz.Count(); i++)
                    {
                        var quizEntity = listQuiz[i];

                        quizFamilies.Add(new QuizFamily()
                        {
                            FamilyId = familyEntity._id.ToString(),
                            QuizId = quizEntity._id.ToString(),
                            HolderName = familyEntity.Holder.Name,
                            HolderCpf = familyEntity.Holder.Cpf,
                            HolderNumber = familyEntity.Holder.Number,
                            TypeStatus = TypeStatus.NaoRespondido,
                            TypeQuiz = quizEntity.TypeQuiz,
                            Title = quizEntity.Title,
                        });
                    }

                    const int limit = 250;
                    var registred = 0;
                    var index = 0;

                    while (quizFamilies.Count() > registred)
                    {
                        var itensToRegister = quizFamilies.Skip(limit * index).Take(limit).ToList();

                        if (itensToRegister.Count() > 0)
                            await _quizFamilyRepository.CreateAsync(itensToRegister);
                        registred += limit;
                        index++;
                    }

                    quizFamilies = quizFamilies = await _quizFamilyRepository.FindByAsync(x => x.DataBlocked == null && x.FamilyId == userId).ConfigureAwait(false) as List<QuizFamily>;
                }

                if (quizFamilies.Count() > 0)
                {
                    var listQuiz = await _quizRepository.FindIn(x => x.TypeQuiz == typeQuiz && x.DataBlocked == null, "_id", quizFamilies.Select(x => ObjectId.Parse(x.QuizId)).ToList(), Builders<Quiz>.Sort.Descending(x => x._id)) as List<Quiz>;
                    if (listQuiz.Count() == 0)
                        return Ok(Utilities.ReturnSuccess(DefaultMessages.AnyQuiz, new List<object>()));

                    _quizViewModel = _mapper.Map<List<QuizViewModel>>(listQuiz);

                    for (int i = 0; i < _quizViewModel.Count(); i++)
                    {
                        var quizfamilyEntity = quizFamilies.Find(x => x.QuizId == _quizViewModel[i].Id.ToString());
                        if (quizfamilyEntity != null)
                        {
                            _quizViewModel[i].TypeStatus = quizfamilyEntity.TypeStatus;
                            _quizViewModel[i].Created = quizfamilyEntity.Created;
                        }
                    }
                }

                return Ok(Utilities.ReturnSuccess(data: _quizViewModel.OrderByDescending(x => x.Created)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTA OS QUESTIONÁRIOS POR FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetQuizByFamilyId")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQuizByFamilyId([FromQuery] string familyId, TypeQuiz typeQuiz)
        {
            try
            {


                var entity = await _familyRepository.FindByIdAsync(familyId).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));


                var quizFamilies = await _quizFamilyRepository.FindByAsync(x => x.FamilyId == familyId).ConfigureAwait(false) as List<QuizFamily>;


                var entityFamily = await _quizRepository.FindIn(x => x.TypeQuiz == typeQuiz, "_id", quizFamilies.Select(x => ObjectId.Parse(x.QuizId)).ToList(), Builders<Quiz>.Sort.Descending(x => x._id)) as List<Quiz>;
                if (entityFamily.Count(x => x.TypeQuiz == typeQuiz) == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));


                var _quizViewModel = _mapper.Map<List<QuizViewModel>>(entityFamily);

                for (int i = 0; i < _quizViewModel.Count(); i++)
                {
                    var quizfamilyEntity = quizFamilies.Find(x => x.QuizId == _quizViewModel[i].Id.ToString());
                    if (quizfamilyEntity != null)
                        _quizViewModel[i].TypeStatus = quizfamilyEntity.TypeStatus;
                }

                return Ok(Utilities.ReturnSuccess(data: _quizViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
    }
}