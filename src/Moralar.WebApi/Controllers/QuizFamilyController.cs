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
using Moralar.Domain.Services;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Family;
using Moralar.Domain.ViewModels.Question;
using Moralar.Domain.ViewModels.QuestionAnswer;
using Moralar.Domain.ViewModels.Quiz;
using Moralar.Domain.ViewModels.QuizFamily;
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

    public class QuizFamilyController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionDescriptionRepository _questionDescriptionRepository;
        private readonly IQuizFamilyRepository _quizFamilyRepository;
        private readonly IQuestionAnswerRepository _questionAnswerRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly IUtilService _utilService;

        public QuizFamilyController(IMapper mapper, IQuestionRepository questionRepository, IQuizRepository quizRepository, IQuestionDescriptionRepository questionDescriptionRepository, IQuizFamilyRepository quizFamilyRepository, IQuestionAnswerRepository questionAnswerRepository, IFamilyRepository familyRepository, IUtilService utilService)
        {
            _mapper = mapper;
            _questionRepository = questionRepository;
            _quizRepository = quizRepository;
            _questionDescriptionRepository = questionDescriptionRepository;
            _quizFamilyRepository = quizFamilyRepository;
            _questionAnswerRepository = questionAnswerRepository;
            _familyRepository = familyRepository;
            _utilService = utilService;
        }

        /// <summary>
        /// LISTAR QUESTIONARIOS DISPONIBILIZADOS PARA AS FAMILIAS 
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("Available/{page}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(List<QuizFamilyListViewModel>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Available([FromRoute] int page, [FromQuery] string number, [FromQuery] string holderName, [FromQuery] string holderCpf, [FromQuery] TypeStatus? status, [FromQuery] TypeQuiz? typeQuiz, [FromQuery] int limit = 30)
        {
            try
            {
                var builder = Builders<Data.Entities.QuizFamily>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.QuizFamily>>();


                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null));

                if (typeQuiz != null)
                    conditions.Add(builder.Eq(x => x.TypeQuiz, typeQuiz));

                if (string.IsNullOrEmpty(number) == false)
                    conditions.Add(builder.Regex(x => x.HolderNumber, new BsonRegularExpression(new Regex(number, RegexOptions.IgnoreCase))));

                if (string.IsNullOrEmpty(holderName) == false)
                    conditions.Add(builder.Regex(x => x.HolderName, new BsonRegularExpression(new Regex(holderName, RegexOptions.IgnoreCase))));

                if (string.IsNullOrEmpty(holderCpf) == false)
                    conditions.Add(builder.Regex(x => x.HolderCpf, new BsonRegularExpression(new Regex(holderCpf.OnlyNumbers(), RegexOptions.IgnoreCase))));


                if (status != null)
                    conditions.Add(builder.Where(x => x.TypeStatus == status));


                var listQuizFamily = await _quizFamilyRepository.GetCollectionAsync().FindSync(builder.And(conditions), new FindOptions<QuizFamily>()
                {
                    Sort = Builders<QuizFamily>.Sort.Descending(x => x.Created),
                    Skip = (page - 1) * limit,
                    Limit = limit,
                    Collation = new Collation("en", strength: CollationStrength.Primary)
                }).ToListAsync();

                var response = _mapper.Map<List<QuizFamilyListViewModel>>(listQuizFamily);

                return Ok(Utilities.ReturnSuccess(data: response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
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
        [HttpPost("Available/LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(List<QuizFamilyListViewModel>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadDataQuizAvailable([FromForm] DtParameters model, [FromForm] string number, [FromForm] string title, [FromForm] string holderName, [FromForm] string holderCpf, [FromForm] TypeStatus? status, [FromForm] TypeQuiz? typeQuiz)
        {

            //
            var response = new DtResult<QuizFamilyListViewModel>();
            try
            {
                var builder = Builders<Data.Entities.QuizFamily>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.QuizFamily>>();

                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null));

                if (string.IsNullOrEmpty(title) == false)
                    conditions.Add(builder.Regex(x => x.Title, new BsonRegularExpression(new Regex(title, RegexOptions.IgnoreCase))));

                if (typeQuiz != null)
                    conditions.Add(builder.Eq(x => x.TypeQuiz, typeQuiz));

                if (string.IsNullOrEmpty(number) == false)
                    conditions.Add(builder.Regex(x => x.HolderNumber, new BsonRegularExpression(new Regex(number, RegexOptions.IgnoreCase))));

                if (string.IsNullOrEmpty(holderName) == false)
                    conditions.Add(builder.Regex(x => x.HolderName, new BsonRegularExpression(new Regex(holderName, RegexOptions.IgnoreCase))));

                if (string.IsNullOrEmpty(holderCpf) == false)
                    conditions.Add(builder.Regex(x => x.HolderCpf, new BsonRegularExpression(new Regex(holderCpf.OnlyNumbers(), RegexOptions.IgnoreCase))));


                if (status != null)
                    conditions.Add(builder.Where(x => x.TypeStatus == status));


                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _quizFamilyRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.QuizFamily>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.QuizFamily>.Sort.Ascending(sortColumn);

                var retorno = await _quizFamilyRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, columns) as List<QuizFamily>;

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                    ? (int)await _quizFamilyRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                    : totalRecords;

                response.Data = _mapper.Map<List<QuizFamilyListViewModel>>(retorno);
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
        /// LISTA TODOS OS QUESTIONÁRIOS DISPONIBILIZADOS PARA A FAMÍLIA
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetAllQuiz")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllQuiz()
        {
            try
            {

                var quizFamily = await _quizFamilyRepository.FindAllAsync().ConfigureAwait(false) as List<QuizFamily>;

                if (quizFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizFamilyNotFound));

                var _quizViewModel = _mapper.Map<List<QuizFamilyListViewModel>>(quizFamily);


                for (int i = 0; i < _quizViewModel.Count; i++)
                {
                    var item = _quizViewModel[i];

                    var entityQuiz = await _quizRepository.FindByIdAsync(item.QuizId).ConfigureAwait(false);

                    _quizViewModel[i].Title = entityQuiz.Title;
                }


                return Ok(Utilities.ReturnSuccess(data: _quizViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }
        /// <summary>
        /// DETALHES DO QUIZ DISPONIBILIZADO PARA O MORADOR
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

                var quizFamily = await _quizFamilyRepository.FindByIdAsync(id).ConfigureAwait(false);
                if (quizFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizFamilyNotFound));

                var family = await _familyRepository.FindByIdAsync(quizFamily.FamilyId).ConfigureAwait(false);
                if (family == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var questionAnswer = await _questionAnswerRepository.FindByAsync(x => x.FamilyId == family._id.ToString()).ConfigureAwait(false);
                if (questionAnswer.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.AnswerNotFound));

                var entityQuiz = await _quizRepository.FindByIdAsync(quizFamily.QuizId).ConfigureAwait(false);
                if (entityQuiz == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuizNotFound));

                var question = await _questionRepository.FindByAsync(x => x.QuizId == entityQuiz._id.ToString()).ConfigureAwait(false) as List<Question>;
                if (question.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuestionNotFound));


                var _quizViewModel = _mapper.Map<QuizDetailViewModel>(entityQuiz);

                var questionDescription = await _questionDescriptionRepository.FindIn("QuestionId", question.Select(x => ObjectId.Parse(x._id.ToString())).ToList()) as List<QuestionDescription>;
                for (int i = 0; i < question.Count(); i++)
                {
                    _quizViewModel.QuestionViewModel.Add(_mapper.Map<QuestionViewModel>(question[i]));
                    foreach (var item in questionDescription.Where(x => x.QuestionId == question[i]._id.ToString()))
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
                List<QuestionAnswerListViewModel> listAnswers = new List<QuestionAnswerListViewModel>();
                var answers = await _questionAnswerRepository.FindAllAsync().ConfigureAwait(false) as List<QuestionAnswer>;

                var response = new List<QuestionAnswerListViewModel>();

                for (int i = 0; i < question.Count; i++)
                {
                    var item = question[i];

                    var itemAnswers = answers.Where(x => x.Questions.Count(c => c.QuestionId == item._id.ToString()) > 0);

                    response.Add(new QuestionAnswerListViewModel()
                    {

                        FamilyNumber = answers[i].FamilyNumber,
                        FamilyHolderName = answers[i].FamilyHolderName,
                        FamilyHolderCpf = answers[i].FamilyHolderCpf,
                        Title = entityQuiz.Title,
                        Date = answers[i].Created.Value,
                        Question = item.NameQuestion
                    });
                }
                return Ok(Utilities.ReturnSuccess(data: _quizViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
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
        [HttpPost("Available/Export")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Export([FromForm] DtParameters model, [FromForm] string title, [FromForm] string number, [FromForm] string holderName, [FromForm] string holderCpf, [FromForm] TypeStatus? status, [FromForm] TypeQuiz? typeQuiz)
        {
            var response = new DtResult<QuizFamilyExportViewModel>();
            try
            {
                var conditions = new List<FilterDefinition<Data.Entities.QuizFamily>>();
                var builder = Builders<Data.Entities.QuizFamily>.Filter;

                conditions.Add(builder.Where(x => x.Created != null && x.Disabled == null));

                if (string.IsNullOrEmpty(title) == false)
                    conditions.Add(builder.Regex(x => x.Title, new BsonRegularExpression(new Regex(title, RegexOptions.IgnoreCase))));

                if (typeQuiz != null)
                    conditions.Add(builder.Eq(x => x.TypeQuiz, typeQuiz));

                if (string.IsNullOrEmpty(number) == false)
                    conditions.Add(builder.Regex(x => x.HolderNumber, new BsonRegularExpression(new Regex(number, RegexOptions.IgnoreCase))));

                if (string.IsNullOrEmpty(holderName) == false)
                    conditions.Add(builder.Regex(x => x.HolderName, new BsonRegularExpression(new Regex(holderName, RegexOptions.IgnoreCase))));

                if (string.IsNullOrEmpty(holderCpf) == false)
                    conditions.Add(builder.Regex(x => x.HolderCpf, new BsonRegularExpression(new Regex(holderCpf.OnlyNumbers(), RegexOptions.IgnoreCase))));

                if (status != null)
                    conditions.Add(builder.Where(x => x.TypeStatus == status));

                var condition = builder.And(conditions);
                var fileName = "Questionarios disponibilizados_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
                var allData = await _quizFamilyRepository.GetCollectionAsync().FindSync(condition, new FindOptions<Data.Entities.QuizFamily>() { }).ToListAsync();

                var listOnlyQuiz = await _quizRepository.FindIn("_id", allData.Select(x => ObjectId.Parse(x.QuizId.ToString())).ToList()) as List<Quiz>;
                var listQuizFamily = _mapper.Map<List<QuizFamilyExportViewModel>>(allData);
                for (int i = 0; i < listQuizFamily.Count(); i++)
                {
                    listQuizFamily[i].Title = listOnlyQuiz.Find(x => x._id == ObjectId.Parse(allData[i].QuizId))?.Title;
                }

                var path = UtilService.getFilePathFromServer("ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);
                var listViewModel = _mapper.Map<List<QuizFamilyExportViewModel>>(listQuizFamily);//(allData);
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
    }
}