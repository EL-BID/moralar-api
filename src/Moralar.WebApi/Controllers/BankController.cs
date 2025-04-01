using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moralar.Data.Entities;
using Moralar.Domain.Services;
using Moralar.Domain.ViewModels;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class BankController : Controller
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;

        public BankController(IBankRepository bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// LISTA DE BANCOS DISPONIVEIS PARA CADASTRO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var listBank = await _bankRepository.FindAllAsync(Builders<Bank>.Sort.Ascending(nameof(Bank.Name))).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<IEnumerable<BankViewModel>>(listBank)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LISTA DE BANCOS DISPONIVEIS PARA CADASTRO (IUGU)
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> List()
        {
            try
            {
                var listBank = UtilService.GetJsonData("banks.json");

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<BankViewModel>>(listBank)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// LISTA DE BANCOS DO BRASIL
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListBrazilian()
        {
            try
            {
                var listBank = UtilService.GetJsonData("brazilian-banks.json");

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<List<BankViewModel>>(listBank)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


    }
}