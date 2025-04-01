using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moralar.Domain;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.Domain.Services;
using Moralar.Domain.Services.Interface;

namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class CityController : Controller
    {
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly IUtilService _utilService;

        public CityController(ICityRepository cityRepository, IStateRepository stateRepository, IMapper mapper, IUtilService utilService)
        {
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _mapper = mapper;
            _utilService = utilService;
        }





        /// <summary>
        /// LISTAR CIDADES POR ESTADOS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{stateId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get([FromRoute] string stateId)
        {
            try
            {
                var cities = UtilService.GetJsonData("cities-br.json");

                var listCity = cities
                    .Where(x => x["stateId"].ToString().Equals(stateId))
                    .OrderBy(x => x["name"].ToString())
                    .ToList();

                return Ok(Utilities.ReturnSuccess(data: listCity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro(responseList: true));
            }
        }

        /// <summary>
        /// LISTAR TODOS PAISES
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ListCountry")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListCountry()
        {
            try
            {
                var listState = UtilService.GetJsonData("list-countries.json");

                return Ok(Utilities.ReturnSuccess(data: listState));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LISTAR ESTADOS 
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ListState")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListState([FromQuery] string? countryId)
        {
            try
            {
                var listState = UtilService.GetJsonData("list-state-br.json", true);

                return Ok(Utilities.ReturnSuccess(data: listState));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// BUSCAR INFORMAÇÕES DE DETERMINADO CEP
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetInfoFromZipCode/{zipCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInfoFromZipCode([FromRoute] string zipCode)
        {
            try
            {
                if (string.IsNullOrEmpty(zipCode))
                    return BadRequest(Utilities.ReturnErro("Informe o CEP"));

                var response = await _utilService.GetInfoFromZipCode(zipCode);

                if (response == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ZipCodeNotFound));


                return Ok(Utilities.ReturnSuccess(data: response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

    }
}