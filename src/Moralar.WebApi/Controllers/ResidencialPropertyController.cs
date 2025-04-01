using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using MimeTypes.Core;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using Moralar.Data.Entities;
using Moralar.Data.Entities.Auxiliar;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Property;
using Moralar.Domain.ViewModels.ResidencialProperty;
using Moralar.Repository.Interface;

using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;
using Moralar.Domain.Services;
using System.Text;

namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class ResidencialPropertyController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IUtilService _utilService;
        private readonly ISenderMailService _senderMailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResidencialPropertyRepository _residencialPropertyRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly IPropertiesInterestRepository _propertiesInterestRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public ResidencialPropertyController(IMapper mapper, IUtilService utilService, ISenderMailService senderMailService, IHttpContextAccessor httpContextAccessor, IResidencialPropertyRepository residencialPropertyRepository, IFamilyRepository familyRepository, IPropertiesInterestRepository propertiesInterestRepository, IScheduleRepository scheduleRepository)
        {
            _mapper = mapper;
            _utilService = utilService;
            _senderMailService = senderMailService;
            _httpContextAccessor = httpContextAccessor;
            _residencialPropertyRepository = residencialPropertyRepository;
            _familyRepository = familyRepository;
            _propertiesInterestRepository = propertiesInterestRepository;
            _scheduleRepository = scheduleRepository;
        }




        /// <summary>
        /// BLOQUEAR / DESBLOQUEAR PROPRIEDADE
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
            var typeAction = model.Block == true ? TypeAction.Block : TypeAction.UnBlock;
            try
            {
                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.TargetId));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entity = await _residencialPropertyRepository.FindByIdAsync(model.TargetId);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));
                entity.DataBlocked = model.Block ? DateTimeOffset.Now.ToUnixTimeSeconds() : (long?)null;
                entity.Reason = model.Block ? model.Reason : null;

                await _residencialPropertyRepository.UpdateAsync(entity);
                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, typeAction, TypeResposible.UserAdminstratorGestor, $"Bloqueio do imóvel {entity._id}", Request.GetUserId(), Request.GetUserName()?.Value, model.TargetId);
                return Ok(Utilities.ReturnSuccess(model.Block ? "Bloqueado com sucesso" : "Desbloqueado com sucesso"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Familia, typeAction, TypeResposible.UserAdminstratorGestor, $"Não foi possível bloquer o imóvel", Request.GetUserId(), Request.GetUserName()?.Value, model.TargetId, "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTAGEM DOS RESIDÊNCIAS
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
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] string code, [FromForm] string status, [FromForm] int availableForSale, [FromForm] TypeStatusResidencial? typeStatusResidencialProperty)
        {
            var response = new DtResult<ResidencialPropertyViewModel>();
            try
            {
                var builder = Builders<Data.Entities.ResidencialProperty>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.ResidencialProperty>>();

                conditions.Add(builder.Where(x => x.Created != null));

                if (typeStatusResidencialProperty != null)
                    conditions.Add(builder.Eq(x => x.TypeStatusResidencialProperty, typeStatusResidencialProperty));

                if (string.IsNullOrEmpty(code) == false)
                    conditions.Add(builder.Where(x => x.Code.ToUpper() == code.ToUpper()));

                if (string.IsNullOrEmpty(status) == false)
                    if (status == "0")
                        conditions.Add(builder.Where(x => x.DataBlocked == null));
                    else if (status == "1")
                        conditions.Add(builder.Where(x => x.DataBlocked != null));
                //var condition = builder.And(conditions);

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _residencialPropertyRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.ResidencialProperty>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.ResidencialProperty>.Sort.Ascending(sortColumn);

                var retorno = await _residencialPropertyRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                    ? (int)await _residencialPropertyRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                    : totalRecords;

                response.Data = _mapper.Map<List<ResidencialPropertyViewModel>>(retorno);
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
        /// DETALHES DA PROPRIEDADE
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
                if (ObjectId.TryParse(id, out var unused) == false)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                var entity = await _residencialPropertyRepository.FindByIdAsync(id).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));

                var viewmodelData = _mapper.Map<ResidencialPropertyViewModel>(entity);

                /* Quantidade de famílias interessadas */
                viewmodelData.InterestedFamilies = await _propertiesInterestRepository.CountAsync(x => x.ResidencialPropertyId == id).ConfigureAwait(false);


                return Ok(Utilities.ReturnSuccess(data: viewmodelData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// DASHBOARD
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("Dashboard")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                int amount = await _familyRepository.CountAsync(x => x.Created != null).ConfigureAwait(false);
                decimal amoutReuniaoPgm = await _scheduleRepository.CountAsync(x => x.TypeSubject == TypeSubject.ReuniaoPGM).ConfigureAwait(false);
                decimal amoutChooseProperty = await _scheduleRepository.CountAsync(x => x.TypeSubject == TypeSubject.EscolhaDoImovel).ConfigureAwait(false);
                decimal amoutChangeProperty = await _scheduleRepository.CountAsync(x => x.TypeSubject == TypeSubject.Mudanca).ConfigureAwait(false);
                decimal amoutPosMudanca = await _scheduleRepository.CountAsync(x => x.TypeSubject == TypeSubject.AcompanhamentoPosMudança).ConfigureAwait(false);
                int availableForSale = await _residencialPropertyRepository.CountAsync(x => x.TypeStatusResidencialProperty == TypeStatusResidencial.AEscolher).ConfigureAwait(false);
                int residencialPropertySaled = await _residencialPropertyRepository.CountAsync(x => x.TypeStatusResidencialProperty == TypeStatusResidencial.Vendido).ConfigureAwait(false);

                decimal percentageAmoutReuniaoPgm = amount == 0 ? 0 : amoutReuniaoPgm / decimal.Parse(amount.ToString());
                decimal percentageAmoutChooseProperty = amount == 0 ? 0 : amoutChooseProperty / decimal.Parse(amount.ToString());
                decimal percentageAmoutChangeProperty = amount == 0 ? 0 : amoutChangeProperty / decimal.Parse(amount.ToString());
                decimal percentageAmoutPosMudanca = amount == 0 ? 0 : amoutPosMudanca / decimal.Parse(amount.ToString());
                var vwDashBoard = new ResidencialPropertyDashboardViewModel()
                {
                    AmountFamilies = amount,
                    AvailableForSale = availableForSale,
                    ResidencialPropertySaled = residencialPropertySaled,
                    PercentageAmoutChangeProperty = percentageAmoutChangeProperty,
                    PercentageAmoutChooseProperty = percentageAmoutChooseProperty,
                    PercentageAmoutPosMudanca = percentageAmoutPosMudanca,
                    PercentageAmoutReuniaoPgm = percentageAmoutReuniaoPgm
                };
                return Ok(Utilities.ReturnSuccess(data: vwDashBoard));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTA TODAS PROPRIEDADES
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetAllBy")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllBy([FromQuery] string code, [FromQuery] int status, [FromQuery] int availableForSale)
        {
            try
            {
                var userId = Request.GetUserId();
                if (!await _scheduleRepository.CheckByAsync(x => x.FamilyId == userId && x.TypeSubject == TypeSubject.EscolhaDoImovel).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotInChooseProperty, responseList: true));

                var builder = Builders<Data.Entities.ResidencialProperty>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.ResidencialProperty>>();

                conditions.Add(builder.Where(x => x.Created != null));
                if (!string.IsNullOrEmpty(code))
                    conditions.Add(builder.Where(x => x.Code.ToUpper() == code.ToUpper()));
                if (status == 0)
                    conditions.Add(builder.Where(x => x.DataBlocked == null));
                else if (status == 1)
                    conditions.Add(builder.Where(x => x.DataBlocked != null));


                var condition = builder.And(conditions);
                var listResidencialEntity = await _residencialPropertyRepository.GetCollectionAsync().FindSync(condition, new FindOptions<Data.Entities.ResidencialProperty>() { }).ToListAsync();

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<ResidencialPropertyViewModel>>(listResidencialEntity)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTA AS PROPRIEDADES COM FILTRO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetResidencialByFilter")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetResidencialByFilter([FromQuery] ResidencialPropertyFilterViewModel model)
        {
            try
            {
                double valueMaximumPurchase = 0;
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredentials));

                var familyEntity = await _familyRepository.FindByIdAsync(userId);
                if (familyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                if (await _scheduleRepository.CheckByAsync(x => x.FamilyId == userId && x.TypeSubject == TypeSubject.EscolhaDoImovel && x.TypeScheduleStatus == TypeScheduleStatus.Confirmado) == false)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ScheduleNotInChooseProperty, responseList: true));

                valueMaximumPurchase = (double)((familyEntity.Financial?.MaximumPurchase ?? 0) + (familyEntity.Financial?.IncrementValue ?? 0));

                var builder = Builders<Data.Entities.ResidencialProperty>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.ResidencialProperty>>();


                var conditionOlds = new List<IMongoQuery>();

                conditions.Add(builder.Where(x => x.Created != null && x.DataBlocked == null && x.TypeStatusResidencialProperty != TypeStatusResidencial.Vendido));

                conditions.Add(builder.Gte(x => x.ResidencialPropertyFeatures.PropertyValue, (double)familyEntity.Financial.PropertyValueForDemolished));
                conditions.Add(builder.Lte(x => x.ResidencialPropertyFeatures.PropertyValue, valueMaximumPurchase));

                conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.Created != null && x.DataBlocked == null && x.TypeStatusResidencialProperty != TypeStatusResidencial.Vendido));

                conditionOlds.Add(Query<ResidencialProperty>.GTE(x => x.ResidencialPropertyFeatures.PropertyValue, (double)familyEntity.Financial.PropertyValueForDemolished));
                conditionOlds.Add(Query<ResidencialProperty>.LTE(x => x.ResidencialPropertyFeatures.PropertyValue, valueMaximumPurchase));
                //conditions.Add(builder.Lte(x => x.ResidencialPropertyFeatures.PropertyValue, (double)familyEntity.Financial.MaximumPurchase));

                if (model.TypeProperty != null)
                {
                    conditions.Add(builder.Eq(x => x.ResidencialPropertyFeatures.TypeProperty, model.TypeProperty));
                    conditionOlds.Add(Query<ResidencialProperty>.EQ(x => x.ResidencialPropertyFeatures.TypeProperty, model.TypeProperty));
                }

                if (model.StartSquareFootage > 0 && model.EndSquareFootage > 0)
                {

                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.SquareFootage >= model.StartSquareFootage && x.ResidencialPropertyFeatures.SquareFootage <= model.EndSquareFootage));// && x.ResidencialPropertyFeatures.SquareFootage <= model.EndSquareFootage
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.SquareFootage >= model.StartSquareFootage && x.ResidencialPropertyFeatures.SquareFootage <= model.EndSquareFootage));// && x.ResidencialPropertyFeatures.SquareFootage <= model.EndSquareFootage
                }

                if (model.StartCondominiumValue > 0 && model.EndCondominiumValue > 0)
                {

                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.CondominiumValue >= model.StartCondominiumValue && x.ResidencialPropertyFeatures.CondominiumValue <= model.EndCondominiumValue));
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.CondominiumValue >= model.StartCondominiumValue && x.ResidencialPropertyFeatures.CondominiumValue <= model.EndCondominiumValue));
                }

                if (model.StartIptuValue > 0 && model.EndIptuValue > 0)
                {
                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.IptuValue >= model.StartIptuValue && x.ResidencialPropertyFeatures.IptuValue <= model.EndIptuValue));// && x.ResidencialPropertyFeatures.SquareFootage <= model.EndSquareFootage
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.IptuValue >= model.StartIptuValue && x.ResidencialPropertyFeatures.IptuValue <= model.EndIptuValue));// && x.ResidencialPropertyFeatures.SquareFootage <= model.EndSquareFootage

                }

                if (model.StartNumberOfBedrooms > 0 && model.EndNumberOfBedrooms > 0)
                {
                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.NumberOfBedrooms >= model.StartNumberOfBedrooms && x.ResidencialPropertyFeatures.NumberOfBedrooms <= model.EndNumberOfBedrooms));
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.NumberOfBedrooms >= model.StartNumberOfBedrooms && x.ResidencialPropertyFeatures.NumberOfBedrooms <= model.EndNumberOfBedrooms));

                }


                if (model.HasGarage != null)
                {
                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.HasGarage == model.HasGarage.GetValueOrDefault()));
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.HasGarage == model.HasGarage.GetValueOrDefault()));

                }

                if (model.HasAccessLadder != null)
                {
                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.HasAccessLadder == model.HasAccessLadder.GetValueOrDefault()));
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.HasAccessLadder == model.HasAccessLadder.GetValueOrDefault()));
                }

                if (model.HasAccessRamp != null)
                {
                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.HasAccessRamp == model.HasAccessRamp.GetValueOrDefault()));
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.HasAccessRamp == model.HasAccessRamp.GetValueOrDefault()));
                }

                if (model.HasAdaptedToPcd != null)
                {
                    conditions.Add(builder.Where(x => x.ResidencialPropertyFeatures.HasAdaptedToPcd == model.HasAdaptedToPcd.GetValueOrDefault()));
                    conditionOlds.Add(Query<ResidencialProperty>.Where(x => x.ResidencialPropertyFeatures.HasAdaptedToPcd == model.HasAdaptedToPcd.GetValueOrDefault()));

                }

                // if (model.Lat != null && model.Lng != null)
                //     conditions.Add(builder.Near(x => x.Position, model.Lat.GetValueOrDefault(), model.Lng.GetValueOrDefault()));

                var condition = builder.And(conditions);

                var listEntity = model.Lat != null && model.Lng != null
                               ? await _residencialPropertyRepository.FindByNearWithDistanceAsync(model.Lat.GetValueOrDefault(), model.Lng.GetValueOrDefault(), 100000, propertyIndex: nameof(ResidencialProperty.Position), distanceProperty: "Distance", queries: conditionOlds)
                               : await _residencialPropertyRepository.GetCollectionAsync().FindSync(condition, new FindOptions<Data.Entities.ResidencialProperty>() { }).ToListAsync();

                if (listEntity.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.AnyResidencialProperty));

                var listInterest = await _propertiesInterestRepository.FindIn(nameof(PropertiesInterest.ResidencialPropertyId), listEntity.Select(x => x._id.ToString()).ToList());
                var response = _mapper.Map<List<ResidencialProperty>, List<ResidencialPropertyViewModel>>(listEntity, opt => opt.AfterMap((src, dest) =>
                {
                    for (int i = 0; i < src.Count(); i++)
                    {
                        dest[i].InterestedFamilies = listInterest.LongCount(x => x.ResidencialPropertyId == dest[i].Id);
                    }
                }));

                return Ok(Utilities.ReturnSuccess(data: response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }

        /// <summary>
        /// REGISTRAR NOVA PROPRIEDADE
        /// </summary>
        /// <remarks>
        ///OBJ DE ENVIO
        ///         POST
        ///{
        ///  "code": "fasdfasd",
        ///  "photo": [
        ///    "1.jpg",
        ///    "2.jpg",
        ///    "3.jpg"
        ///  ],
        ///  "project": "project",
        /// "residencialPropertyAdress": {
        ///  "cep": "string",
        ///  "streetAddress": "streetAddress",
        ///  "number": "number",
        ///  "cityName": "cityName",
        ///  "cityId": "cityId",
        ///  "stateName": "stateName",
        ///  "stateUf": "stateUf",
        ///  "stateId": "stateId",
        ///  "neighborhood": "neighborhood",
        ///  "complement": "complement",
        ///  "location": "location",
        ///  "latitude": 0,
        ///  "longitude": 0
        ///},
        ///  "residencialPropertyFeatures": {
        ///    "propertyValue": 200,
        ///    "typeProperty": 0,// Enum
        ///    "squareFootage": 1,
        ///    "condominiumValue": 22,
        ///    "iptuValue": 333,
        ///    "neighborhood": "neighborhood",
        ///    "numberFloors": 20,
        ///    "floorLocation": 2,
        ///    "hasElavator": true,
        ///    "numberOfBedrooms": 1,
        ///    "numberOfBathrooms": 1,
        ///    "hasServiceArea": true,
        ///    "hasGarage": true,
        ///    "hasYard": true,
        ///    "hasCistern": true,
        ///    "hasWall": true,
        ///    "hasAccessLadder": true,
        ///    "hasAccessRamp": true,
        ///    "hasAdaptedToPcd": true,
        ///    "propertyRegularization": 1,//Enum
        ///    "typeGasInstallation": 1 //Enum
        ///  }
        ///}
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
        public async Task<IActionResult> Register([FromBody] ResidencialPropertyViewModel model)
        {
            //var claim = Util.SetRole(TypeProfile.Profile);
            //var typeAction = string.IsNullOrEmpty(model.Id) ? TypeAction.Register : TypeAction.Change;
            try
            {
                if (model.Photo.Count() == 0 || model.Photo.Count > 15)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.AmountPhoto));

                var ignoreValidation = new List<string>();

                ignoreValidation.Add("Id");

                var isInvalidState = ModelState.ValidModelState(ignoreValidation.ToArray());

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                for (int i = 0; i < model.Photo.Count(); i++)
                    model.Photo[i].ImageUrl = model.Photo[i].ImageUrl.SetPathImage();
                
                model.Project = model.Project.SetPathImage();
                var entity = _mapper.Map<Data.Entities.ResidencialProperty>(model);
                entity.TypeStatusResidencialProperty = TypeStatusResidencial.AEscolher;
                entity.Position = new List<double>() { model.ResidencialPropertyAdress.Latitude, model.ResidencialPropertyAdress.Longitude };

                var entityId = await _residencialPropertyRepository.CreateAsync(entity).ConfigureAwait(false);




                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Cadastro de novo imóvel {entity.Code}", Request.GetUserId(), Request.GetUserName()?.Value, entityId, "");

                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar novo imóvel", Request.GetUserId(), Request.GetUserName()?.Value, "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// ATUALIZAR IMÓVEL
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
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
        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] ResidencialPropertyViewModel model)
        {
            try
            {
                var validOnly = await Utilities.GetFieldsFromBodyAsync(Utilities.ConvertObjectToDictionary(model));

                var isInvalidState = ModelState.ValidModelStateOnlyFields(validOnly);

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);


                var residencialPropertyEntity = await _residencialPropertyRepository.FindByIdAsync(id);

                if (residencialPropertyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));

                residencialPropertyEntity.SetIfDifferent(model, validOnly);

                if (Util.CheckHasField(validOnly, nameof(model.ResidencialPropertyAdress)))
                {
                    residencialPropertyEntity.ResidencialPropertyAdress = _mapper.Map<ResidencialPropertyAdress>(model.ResidencialPropertyAdress);
                    residencialPropertyEntity.Position = new List<double> { residencialPropertyEntity.ResidencialPropertyAdress.Latitude, residencialPropertyEntity.ResidencialPropertyAdress.Longitude };
                }
                if (Util.CheckHasField(validOnly, nameof(model.ResidencialPropertyAdress)))
                {
                    residencialPropertyEntity.ResidencialPropertyFeatures = _mapper.Map<ResidencialPropertyFeatures>(model.ResidencialPropertyFeatures);
                }
                if (model.Photo.Any())
                {
                    residencialPropertyEntity.Photo = model.Photo.Select(x =>
                    {
                        x.ImageUrl = x.ImageUrl.RemovePathImage();
                        Data.Entities.Auxiliar.ResidencialPropertyPhoto newPhoto = new Data.Entities.Auxiliar.ResidencialPropertyPhoto
                        {
                            ImageUrl = x.ImageUrl,
                            Description = x.Description
                           
                        };
                        return newPhoto;
                    }).ToList();
                }

                if (model.Project.Length > 0)
                {
                    residencialPropertyEntity.Project = model.Project;
                }

                residencialPropertyEntity.Description = model.Description;

                residencialPropertyEntity = await _residencialPropertyRepository.UpdateAsync(residencialPropertyEntity);

                var viewmodelData = _mapper.Map<ResidencialPropertyViewModel>(residencialPropertyEntity);

                /* Quantidade de famílias interessadas */
                viewmodelData.InterestedFamilies = await _propertiesInterestRepository.CountAsync(x => x.ResidencialPropertyId == id).ConfigureAwait(false);


                return Ok(Utilities.ReturnSuccess(DefaultMessages.Updated, viewmodelData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// GESTOR REALIZA A VENDA PARA DETERMINADA PESSOA
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ContemplateFamilyInTheResidence")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ContemplateFamilyInTheResidence([FromBody] ResidencialPropertyChoicePropertyViewModel model)
        {
            try
            {
                var residencialPropertyEntity = await _residencialPropertyRepository.FindByIdAsync(model.ResidencialPropertyId).ConfigureAwait(false);
                if (residencialPropertyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));

                residencialPropertyEntity.FamiliIdResidencialContemplate = model.FamiliIdResidencialContemplate ?? null;

                await _residencialPropertyRepository.UpdateAsync(residencialPropertyEntity).ConfigureAwait(false);

                if (model.FamiliIdResidencialContemplate != null)
                {
                    var familyEntity = await _familyRepository.FindByIdAsync(model.FamiliIdResidencialContemplate).ConfigureAwait(false);
                    if (familyEntity == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                    if (model.FamiliIdResidencialContemplate != null)
                    {
                        await _utilService.sentNotificationOfContemplateToEquipeTTSAsync(familyEntity, residencialPropertyEntity._id.ToString(), residencialPropertyEntity.Code);
                    }
                }

                return Ok(Utilities.ReturnSuccess(data: $"{(model.FamiliIdResidencialContemplate != null ? "Contemplado" : "Descontemplado")} com sucesso!"));

            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Familia, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível contemplar/descontemplar nova Família", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// GESTOR REALIZA A VENDA PARA DETERMINADA PESSOA
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ChoiceProperty")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChoiceProperty([FromBody] ResidencialPropertyChoicePropertyViewModel model)
        {
            try
            {
                var residencialPropertyEntity = await _residencialPropertyRepository.FindByIdAsync(model.ResidencialPropertyId).ConfigureAwait(false);
                if (residencialPropertyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));

                var familyEntity = await _familyRepository.FindByIdAsync(model.FamiliIdResidencialChosen).ConfigureAwait(false);
                if (familyEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyNotFound));

                var validOnly = await Utilities.GetFieldsFromBodyAsync(Utilities.ConvertObjectToDictionary(model));


                if (string.IsNullOrEmpty(residencialPropertyEntity.FamiliIdResidencialChosen) == false)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.AlreadySelectedForFamily));

                if (await _residencialPropertyRepository.CheckByAsync(x => x._id != ObjectId.Parse(model.ResidencialPropertyId) && x.FamiliIdResidencialChosen == model.FamiliIdResidencialChosen))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FamilyHasPropertySelected));

                residencialPropertyEntity.FamiliIdResidencialChosen = model.FamiliIdResidencialChosen;
                residencialPropertyEntity.TypeStatusResidencialProperty = TypeStatusResidencial.Vendido;

                await _residencialPropertyRepository.UpdateAsync(residencialPropertyEntity).ConfigureAwait(false);

                var sendInformation = await _propertiesInterestRepository.FindByAsync(x => x.ResidencialPropertyId == model.ResidencialPropertyId && x.FamilyId != model.FamiliIdResidencialChosen).ConfigureAwait(false);

                var removedInterest = sendInformation.Where(x => x.FamilyId != model.FamiliIdResidencialChosen).ToList();


                for (int i = 0; i < removedInterest.Count(); i++)
                {
                    var dataBody = Util.GetTemplateVariables();
                    var item = removedInterest[i];
                    var familyEntityItem = await _familyRepository.FindByIdAsync(item.FamilyId).ConfigureAwait(false);

                    dynamic payloadPush = Util.GetPayloadPush(RouteNotification.System);
                    dynamic settingsPush = Util.GetSettingsPush();

                    string title = "Imóvel vendido";
                    var message = $"<p>Olá {item.HolderName.GetFirstName()}</p>"
                                + $"<p> O imóvel {residencialPropertyEntity.ResidencialPropertyAdress.StreetAddress} foi vendido para outra pessoa. Atualize sua lista de interesses";

                    dataBody.Add("{{ title }}", title);
                    dataBody.Add("{{ message }}", message);

                    var body = _senderMailService.GerateBody("custom", dataBody);

                    var unused2 = Task.Run(async () =>
                    {
                        await _senderMailService.SendMessageEmailAsync("MORALAR", familyEntityItem.Holder.Email, body, title).ConfigureAwait(false);
                    });

                    /*REMOVE AS FAMÍLIAS INTERESSADAS*/
                    await _propertiesInterestRepository.DeleteAsync(x => x.FamilyId == item.FamilyId && x.ResidencialPropertyId == model.ResidencialPropertyId);
                }

                var dataBodyFamily = Util.GetTemplateVariables();

                dataBodyFamily.Add("{{ title }}", "Escolha do imóvel");
                dataBodyFamily.Add("{{ message }}", $"<p>Olá {familyEntity.Holder.Name.GetFirstName()}</p>" +
                                                    $"<p> Você foi escolhido para a compra do imóvel {residencialPropertyEntity.ResidencialPropertyAdress.StreetAddress}.");

                var bodyFamily = _senderMailService.GerateBody("custom", dataBodyFamily);

                var unused = Task.Run(async () =>
                {
                    await _senderMailService.SendMessageEmailAsync("Moralar", familyEntity.Holder.Email, bodyFamily, "Imóvel Escolhido").ConfigureAwait(false);
                });

                bool sendNotify = true;
                ForType forType = ForType.Administrador;
                string titleN = "Um imóvel foi vendido";

                var messageN = new StringBuilder();
                string contentN = "";

                messageN.AppendLine($"O imóvel #{residencialPropertyEntity.Code} ({residencialPropertyEntity.ResidencialPropertyAdress.StreetAddress}) foi vendido para a família {familyEntity.Holder.Number}");
                contentN = messageN.ToString();

                if (sendNotify)
                {
                    await _utilService.SendNotify(titleN, contentN, null, familyEntity.DeviceId, forType, familyEntity._id.ToString(), null, null,
                        UtilService.getModuleMatchSelled(residencialPropertyEntity._id.ToString(), residencialPropertyEntity.Code));
                }

                await _utilService.RegisterLogAction(LocalAction.Familia, TypeAction.Change, TypeResposible.UserAdminstratorGestor, $"Update de nova família {familyEntity.Holder.Name}", "", "", model.Id);//Request.GetUserName()?.Value, Request.GetUserId()

                return Ok(Utilities.ReturnSuccess(data: "Registrado com sucesso!"));

            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.Familia, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível cadastrar nova Família", "", "", "", "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// DELETAR FOTO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        ///         POST
        ///           {
        ///             "id": "6011d02a4c7c9e71c25df866", Id da propriedade
        ///             "name": "" // nome da foto
        ///            }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("DeletePhoto")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePhoto([FromBody] ResidencialPropertyDeletePhotoViewModel model)
        {
            try
            {
                var entity = await _residencialPropertyRepository.FindByIdAsync(model.Id).ConfigureAwait(false);
                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));
                if (entity.Photo.Count() == 0 || entity.Photo.FindIndex(x => x.ImageUrl == model.Name) < 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.PhotoNotFound));

                entity.Photo.RemoveAt(entity.Photo.FindIndex(x => x.ImageUrl == model.Name));

                await _residencialPropertyRepository.UpdateOneAsync(entity).ConfigureAwait(false);

                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, TypeAction.Delete, TypeResposible.UserAdminstratorGestor, $"Remover foto  ", Request.GetUserId(), Request.GetUserName()?.Value, entity._id.ToString());

                return Ok(Utilities.ReturnSuccess(data: "Foto excluída com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, TypeAction.Delete, TypeResposible.UserAdminstratorGestor, $"Não foi possível a foto", Request.GetUserId(), Request.GetUserName()?.Value, model.Id, "", ex);
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// REGISTRA NOVA FOTO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        ///         POST
        ///           {
        ///             "id": "6011d02a4c7c9e71c25df866", Id da propriedade
        ///             "name": "" // nome da foto
        ///            }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("RegisterNewPhoto")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterNewPhoto([FromBody] ResidencialPropertyAddPhoto model)
        {
            try
            {
                var entity = await _residencialPropertyRepository.FindByIdAsync(model.Id).ConfigureAwait(false);
                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ResidencialPropertyNotFound));

                if (model.Photo.ImageUrl.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.AmountPhoto));

                int amoutPhoto = entity.Photo.Count() + 1;
                if (amoutPhoto > 15)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.AmountPhotoInclude));

                model.Photo.ImageUrl.SetPathImage();

                Data.Entities.Auxiliar.ResidencialPropertyPhoto newPhoto = new Data.Entities.Auxiliar.ResidencialPropertyPhoto
                {
                    ImageUrl = model.Photo.ImageUrl,
                    Description = model.Photo.Description
                };

                entity.Photo.Add(newPhoto);

                await _residencialPropertyRepository.UpdateOneAsync(entity).ConfigureAwait(false);

                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Registrar foto  ", Request.GetUserId(), Request.GetUserName()?.Value, entity._id.ToString());

                return Ok(Utilities.ReturnSuccess(data: "Atualizado com sucesso!"));
            }
            catch (Exception ex)
            {
                await _utilService.RegisterLogAction(LocalAction.ResidencialProperty, TypeAction.Register, TypeResposible.UserAdminstratorGestor, $"Não foi possível registrar a foto", Request.GetUserId(), Request.GetUserName()?.Value, model.Id, "", ex);
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

        public async Task<IActionResult> Export([FromForm] DtParameters model, [FromForm] string code, [FromForm] string status, [FromForm] int availableForSale, [FromForm] TypeStatusResidencial? typeStatusResidencialProperty, [FromForm] List<string> id)
        {
            var response = new DtResult<ResidencialPropertyExportViewModel>();
            try
            {
                var builder = Builders<Data.Entities.ResidencialProperty>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.ResidencialProperty>>();

                conditions.Add(builder.Where(x => x._id != null));


                if (id.Count > 0)
                {

                    List<ObjectId> objectIds = id.Select(listId => new ObjectId(listId)).ToList();
                    conditions.Add(builder.In(x => x._id, objectIds));
                }


                if (typeStatusResidencialProperty != null)
                    conditions.Add(builder.Eq(x => x.TypeStatusResidencialProperty, typeStatusResidencialProperty));

                if (string.IsNullOrEmpty(code) == false)
                    conditions.Add(builder.Where(x => x.Code.ToUpper() == code.ToUpper()));

                if (string.IsNullOrEmpty(status) == false)
                    if (status == "0")
                        conditions.Add(builder.Where(x => x.DataBlocked == null));
                    else if (status == "1")
                        conditions.Add(builder.Where(x => x.DataBlocked != null));
                //var condition = builder.And(conditions);

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _residencialPropertyRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.ResidencialProperty>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.ResidencialProperty>.Sort.Ascending(sortColumn);

                var allData = await _residencialPropertyRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, totalRecords, conditions, columns);

                var condition = builder.And(conditions);
                var fileName = "Imoveis_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";

                var path = UtilService.getFilePathFromServer("ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);
                var listViewModel = _mapper.Map<List<ResidencialPropertyExportViewModel>>(allData);
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

        /// <summary>
        /// DOWNLOAD DE ARQUIVO DE EXEMPLO DE IMPORTAÇÃO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ExampleFileImport")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        public async Task<IActionResult> ExampleFileImport()
        {
            try
            {
                var fileName = "Modelo Importar Imoveis.xlsx";
                var path = UtilService.getFilePathFromServer("/Content/ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);
                var listViewModel = new List<ResidencialPropertyImportViewModel>();


                Utilities.ExportToExcel(listViewModel, path, fileName: fileName.Split('.')[0]);
                if (System.IO.File.Exists(fullPathFile) == false)
                    return BadRequest(Utilities.ReturnErro("Ocorreu um erro fazer download do arquivo"));

                var fileBytes = System.IO.File.ReadAllBytes(fullPathFile);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// METODO PARA LISTAR BAIRROS CADASTRADOS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("RegisteredNeighborhoods")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisteredNeighborhoods()
        {
            try
            {
                var listProperty = await _residencialPropertyRepository.FindByAsync(x => x.Disabled == null) as List<ResidencialProperty>;

                var listNeighborhood = listProperty.Select(x => x.ResidencialPropertyAdress.Neighborhood).Where(x => string.IsNullOrEmpty(x) == false).Distinct();

                return Ok(Utilities.ReturnSuccess(data: listNeighborhood));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }

        /// <summary>
        /// IMPORTAR IMOVEIS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("FileImport")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [AllowAnonymous]

        public async Task<IActionResult> FileImport([FromForm] IFormFile file)
        {
            try
            {

                if (file == null || file.Length <= 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FileNotFound));

                var extension = MimeTypeMap.GetExtension(file.ContentType).ToLower();

                if (Util.AcceptedFiles.Count(x => x == extension) == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FileNotAllowed));

                var listEntityViewModel = await Util.ReadAndValidationExcel<ResidencialPropertyImportViewModel>(file);

                if (listEntityViewModel.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ZeroItems));


                var listEntity = _mapper.Map<List<ResidencialProperty>>(Util.formatDataOfResidencialPropertyImportViewModel(listEntityViewModel));


                for (int i = 0; i < listEntity.Count(); i++)
                {
                    listEntity[i].TypeStatusResidencialProperty = TypeStatusResidencial.AEscolher;
                    listEntity[i].Position = new List<double> { listEntity[i].ResidencialPropertyAdress.Latitude, listEntity[i].ResidencialPropertyAdress.Longitude };
                }

                const int limit = 250;
                var registred = 0;
                var index = 0;

                while (listEntity.Count() > registred)
                {
                    var itensToRegister = listEntity.Skip(limit * index).Take(limit).ToList();

                    if (itensToRegister.Count() > 0)
                        await _residencialPropertyRepository.CreateAsync(itensToRegister);
                    registred += limit;
                    index++;
                }

                return Ok(Utilities.ReturnSuccess($"Importação realizada com sucesso, total de {listEntity.Count()} imóvel(is)"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

    }
}