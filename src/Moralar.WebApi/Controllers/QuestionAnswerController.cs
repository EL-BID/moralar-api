using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
    [DisplayName("QuestionController - Comandos para manipulação das fasdfas")]

    public class QuestionAnswerController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionDescriptionRepository _questionDescriptionRepository;
        private readonly IQuestionAnswerRepository _questionAnswerRepository;
        private readonly IQuizFamilyRepository _quizFamilyRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly IUtilService _utilService;

        public QuestionAnswerController(IMapper mapper, IQuestionRepository questionRepository, IQuizRepository quizRepository, IQuestionDescriptionRepository questionDescriptionRepository, IQuestionAnswerRepository questionAnswerRepository, IQuizFamilyRepository quizFamilyRepository, IProfileRepository profileRepository, IFamilyRepository familyRepository, IUtilService utilService)
        {
            _mapper = mapper;
            _questionRepository = questionRepository;
            _quizRepository = quizRepository;
            _questionDescriptionRepository = questionDescriptionRepository;
            _questionAnswerRepository = questionAnswerRepository;
            _quizFamilyRepository = quizFamilyRepository;
            _profileRepository = profileRepository;
            _familyRepository = familyRepository;
            _utilService = utilService;
        }

        /// <summary>
        /// BUSCA AS RESPOSTAS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetResponses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetResponses([FromQuery] string quizId, [FromQuery] string familyId)
        {
            try
            {
                var userId = familyId ?? Request.GetUserId();

                List<QuestionAnswerListViewModel> listAnswers = new List<QuestionAnswerListViewModel>();

                var quiz = await _quizRepository.FindByIdAsync(quizId).ConfigureAwait(false);

                var builderAnswer = Builders<QuestionAnswer>.Filter;
                var conditionsAnswer = new List<FilterDefinition<QuestionAnswer>>();

                conditionsAnswer.Add(builderAnswer.Where(x => x.QuizId == quizId && x.FamilyId == userId));

                var answers = await _questionAnswerRepository.GetCollectionAsync().FindSync(builderAnswer.And(conditionsAnswer)).ToListAsync();


                var builderQuestion = Builders<Question>.Filter;
                var conditionsQuestion = new List<FilterDefinition<Question>>();

                conditionsQuestion.Add(builderQuestion.Eq(x => x.QuizId, quizId));

                var question = await _questionRepository.GetCollectionAsync().FindSync(builderQuestion.And(conditionsQuestion)).ToListAsync();

                //var builderQuestionDescrip = Builders<QuestionDescription>.Filter;
                //var conditionsQuestionDescrip = new List<FilterDefinition<QuestionDescription>>();               


                //var questionsId = answers.SelectMany(x => x.Questions).Select(x => x.QuestionId).ToList();
                //conditionsQuestionDescrip.Add(builderQuestionDescrip.In(x => x.QuestionId, questionsId));


                //var descriptionsId = answers.SelectMany(x => x.Questions).SelectMany(x => x.QuestionDescriptionId).Select(ObjectId.Parse).ToList();
                // conditionsAnswer.Add(builderAnswer.In(x => x._id, descriptionsId));


                //var questionDescription = await _questionDescriptionRepository.GetCollectionAsync().FindSync(builderQuestionDescrip.And(conditionsQuestionDescrip)).ToListAsync();

                var response = new List<QuestionAnswerListViewModel>();

                for (int i = 0; i < question.Count; i++)
                {
                    var item = question[i];

                    var itemAnswers = answers.Where(x => x.Questions.Count(c => c.QuestionId == item._id.ToString()) > 0).ToList();


                    if (itemAnswers.Count() > 0)
                    {
                        response.Add(new QuestionAnswerListViewModel()
                        {
                            Title = quiz.Title,
                            Date = itemAnswers[0].Created.Value,
                            Question = item.NameQuestion,
                            QuestionId = item._id.ToString(),
                            QuizId = item.QuizId,
                            Answers = itemAnswers.Where(x => x.Questions.Count(y => y.QuestionId == item._id.ToString()) > 0).SelectMany(x => x.Questions).Where(y => y.QuestionId == item._id.ToString()).Select(x => x.AnswerDescription).Distinct().ToList(),
                            TypeResponse = item.TypeResponse,
                            FamilyNumber = itemAnswers[0].FamilyNumber,
                            FamilyHolderName = itemAnswers[0].FamilyHolderName,
                            FamilyHolderCpf = itemAnswers[0].FamilyHolderCpf
                        });
                    }
                }

                return Ok(Utilities.ReturnSuccess(data: response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }

        /// <summary>
        /// BUSCA AS RESPOSTAS POR FAMILIA (TTS)
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetResponsesByFamily")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetResponsesByFamily([FromQuery] string familyId, [FromQuery] string quizId)
        {
            try
            {

                List<QuestionAnswerListViewModel> listAnswers = new List<QuestionAnswerListViewModel>();

                var quiz = await _quizRepository.FindByIdAsync(quizId).ConfigureAwait(false);

                var builderAnswer = Builders<QuestionAnswer>.Filter;
                var conditionsAnswer = new List<FilterDefinition<QuestionAnswer>>();

                if (string.IsNullOrEmpty(familyId) == false)
                    conditionsAnswer.Add(builderAnswer.Eq(x => x.FamilyId, familyId));
                if (string.IsNullOrEmpty(quizId) == false)
                    conditionsAnswer.Add(builderAnswer.Eq(x => x.QuizId, quizId));

                if (conditionsAnswer.Count() == 0)
                    conditionsAnswer.Add(builderAnswer.Empty);

                var answers = await _questionAnswerRepository.GetCollectionAsync().FindSync(builderAnswer.And(conditionsAnswer)).ToListAsync();

                var builderQuestion = Builders<Question>.Filter;
                var conditionsQuestion = new List<FilterDefinition<Question>>();

                if (string.IsNullOrEmpty(quizId) == false)
                    conditionsQuestion.Add(builderQuestion.Eq(x => x.QuizId, quizId));

                if (conditionsQuestion.Count() == 0)
                    conditionsQuestion.Add(builderQuestion.Empty);

                var question = await _questionRepository.GetCollectionAsync().FindSync(builderQuestion.And(conditionsQuestion)).ToListAsync();

                var response = new List<QuestionAnswerListViewModel>();

                for (int i = 0; i < question.Count; i++)
                {
                    var item = question[i];

                    var itemAnswers = answers.Where(x => x.Questions.Count(c => c.QuestionId == item._id.ToString()) > 0).ToList();

                    if (itemAnswers.Count() > 0)
                    {
                        response.Add(new QuestionAnswerListViewModel()
                        {
                            Title = quiz.Title,
                            Date = itemAnswers[0].Created.Value,
                            Question = item.NameQuestion,
                            QuestionId = item._id.ToString(),
                            QuizId = item.QuizId,
                            Answers = itemAnswers.Where(x => x.Questions.Count(y => y.QuestionId == item._id.ToString()) > 0).SelectMany(x => x.Questions).Where(y => y.QuestionId == item._id.ToString()).Select(x => x.AnswerDescription).Distinct().ToList(),
                            TypeResponse = item.TypeResponse,
                            FamilyNumber = itemAnswers[0].FamilyNumber,
                            FamilyHolderName = itemAnswers[0].FamilyHolderName,
                            FamilyHolderCpf = itemAnswers[0].FamilyHolderCpf
                        });
                    }
                }

                return Ok(Utilities.ReturnSuccess(data: response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }

        /// <summary>
        /// REGISTRAR UMA RESPOSTA
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///       POST
        ///       {
        ///         "familyId": "string",
        ///         "quizId": "string",
        ///         "questions": {
        ///            "questionDescriptionId": [
        ///               "string"
        ///          ],
        ///          "questionId": "string",
        ///          "answerDescription": "string"
        ///          },
        ///          "responsibleForResponsesId": "string"
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
        public async Task<IActionResult> Register([FromBody] QuestionAnswerRegisterViewModel model)
        {

            try
            {
                var message = $"Registro de nova questão {Request.GetUserName()}";

                var ignoreValidation = new List<string>();
                var isInvalidState = ModelState.ValidModelState(ignoreValidation.ToArray());
                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entityQuestion = await _quizRepository.FindByIdAsync(model.QuizId).ConfigureAwait(false);
                if (entityQuestion == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.QuestionNotFound));

                var entityFamily = await _familyRepository.FindByIdAsync(model.FamilyId).ConfigureAwait(false);
                if (entityFamily == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));
                model.FamilyHolderName = entityFamily.Holder.Name;
                model.FamilyHolderCpf = entityFamily.Holder.Cpf;
                model.FamilyNumber = entityFamily.Holder.Number;


                var profileResponsibleForResponse = new Data.Entities.Profile();
                if (string.IsNullOrEmpty(model.ResponsibleForResponsesId) == false)
                {
                    profileResponsibleForResponse = await _profileRepository.FindByIdAsync(model.ResponsibleForResponsesId).ConfigureAwait(false);
                    if (profileResponsibleForResponse != null)
                    {
                        model.ResponsibleForResponsesCpf = profileResponsibleForResponse.Cpf;
                        model.ResponsibleForResponsesName = profileResponsibleForResponse.Name;
                    }
                }

                var entityNewAnswer = _mapper.Map<QuestionAnswer>(model);
                entityNewAnswer.ResponsibleForResponses = model.ResponsibleForResponsesId == null ? Request.GetUserId() : model.ResponsibleForResponsesId;
                await _questionAnswerRepository.CreateAsync(entityNewAnswer);


                /* Atualiza o status do quiz para respondido */
                _quizFamilyRepository.UpdateMultiple(Query<QuizFamily>.Where(x => x.FamilyId == model.FamilyId && x.QuizId == model.QuizId),
                    new UpdateBuilder<QuizFamily>().Set(x => x.TypeStatus, TypeStatus.Respondido), UpdateFlags.Multi);


                bool sendNotify = true;
                ForType forType = ForType.TTS;
                string titleN = "Foi preenchido um questionário";
                string contentN = null; 
                var messageN = new StringBuilder();


                messageN.AppendLine("<p>Olá equipe TTS, <br/>");
                messageN.AppendLine($"A família {entityFamily.Holder.Number} preencheu o questionário {entityQuestion.Title} </p>");
                contentN = messageN.ToString();

                if (sendNotify)
                {
                    await _utilService.SendNotify(titleN, contentN, null, entityFamily.DeviceId, forType, entityFamily._id.ToString(), null, null,
                        UtilService.getModuleQuestSended(entityQuestion._id.ToString(), entityFamily._id.ToString()));

                }
                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Question, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar/atualizar novo questionário", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }
    }
}