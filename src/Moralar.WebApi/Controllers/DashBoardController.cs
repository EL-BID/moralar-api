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
using Moralar.Domain.ViewModels.Family;
using Moralar.Domain.ViewModels.Question;
using Moralar.Domain.ViewModels.Quiz;
using Moralar.Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;


namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class DashBoardController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IFamilyRepository _familyRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IResidencialPropertyRepository _residencialPropertyRepository;
        private readonly IUtilService _utilService;

        public DashBoardController(IMapper mapper, IFamilyRepository familyRepository, IProfileRepository profileRepository, IResidencialPropertyRepository residencialPropertyRepository, IUtilService utilService)
        {
            _mapper = mapper;
            _familyRepository = familyRepository;
            _profileRepository = profileRepository;
            _residencialPropertyRepository = residencialPropertyRepository;
            _utilService = utilService;
        }




        /// <summary>
        /// LISTA A QUANTIDADE DE FAMÍLIAS CADASTRADAS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("RegisteredFamilies")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisteredFamilies()
        {
            try
            {
                var entity = await _familyRepository.CountAsync(x=> x._id != null).ConfigureAwait(false);
                return Ok(Utilities.ReturnSuccess(data: entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LISTA A QUANTIDADE DE TTS CADASTRADOS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("RegisteredTTS")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisteredTTS()
        {
            try
            {
                var entity = await _profileRepository.CountAsync(x => x.TypeProfile == TypeUserProfile.TTS).ConfigureAwait(false);
                return Ok(Utilities.ReturnSuccess(data: entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTA A QUANTIDADE DE GESTORES CADASTRADOS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("RegisteredManagers")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisteredManagers()
        {
            try
            {
                var entity = await _profileRepository.CountAsync(x => x.TypeProfile == TypeUserProfile.Gestor).ConfigureAwait(false);
                return Ok(Utilities.ReturnSuccess(data: entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// LISTA A QUANTIDADE DE RESIDENCIAS CADASTRADAS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("RegisteredResidencialProperty")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisteredResidencialProperty()
        {
            try
            {
                var entity = await _residencialPropertyRepository.CountAsync(x => x._id != null).ConfigureAwait(false);
                return Ok(Utilities.ReturnSuccess(data: entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

    }

}