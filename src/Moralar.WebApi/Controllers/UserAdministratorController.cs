using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Admin;
using Moralar.Repository.Interface;
using Moralar.WebApi.Services;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;



namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]


    public class UserAdministratorController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IUserAdministratorRepository _userAdministratorRepository;
        private readonly ISenderMailService _senderMailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAdministratorController(IWebHostEnvironment env, IMapper mapper, IUserAdministratorRepository userAdministratorRepository, ISenderMailService senderMailService, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _mapper = mapper;
            _userAdministratorRepository = userAdministratorRepository;
            _senderMailService = senderMailService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Test Server
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("TestServer")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        public IActionResult TestServer()
        {
            return Ok(Utilities.ReturnSuccess("App Moralar is running correctly!"));
        }




        /// <summary>
        /// GET INFO 
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("GetInfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]


        public async Task<IActionResult> GetInfo()
        {
            try
            {
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                var entity = await _userAdministratorRepository.FindByIdAsync(userId).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.UserAdministratorNotFound));

                var vw = _mapper.Map<UserAdministratorViewModel>(entity);
                vw.Password = entity.Password;
                return Ok(Utilities.ReturnSuccess(data: vw));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// GET INFO 
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
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                var entity = await _userAdministratorRepository.FindByIdAsync(id).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.UserAdministratorNotFound));

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<UserAdministratorViewModel>(entity)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// CADASTRO | UPDATE
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "login":"string",
        ///              "name":"string",
        ///              "email":"string",
        ///              "password":"string",
        ///              "level":0
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] UserAdministratorViewModel model)
        {
            var claim = Util.SetRole(TypeProfile.Admin);
            try
            {
                var validOnly = await Utilities.GetFieldsFromBodyAsync(Utilities.ConvertObjectToDictionary(model));

                model.TrimStringProperties();

                if (string.IsNullOrEmpty(model.Id))
                {
                    var isInvalidState = ModelState.ValidModelState(nameof(model.Password));

                    if (isInvalidState != null)
                        return BadRequest(isInvalidState);

                    if (await _userAdministratorRepository.CheckByAsync(x => x.Email == model.Email))
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.EmailInUse));

                    model.TypeProfile = TypeUserProfile.Admin;
                    var entity = _mapper.Map<UserAdministrator>(model);

                    if (string.IsNullOrEmpty(model.Password))
                        entity.Password = Utilities.RandomString(8);

                    await _userAdministratorRepository.CreateAsync(entity).ConfigureAwait(false);
                }
                else
                {
                    var isInvalidState = ModelState.ValidModelStateOnlyFields(validOnly);

                    if (isInvalidState != null)
                        return BadRequest(isInvalidState);

                    var userAdministratorEntity = await _userAdministratorRepository.FindByIdAsync(model.Id).ConfigureAwait(false);

                    if (userAdministratorEntity == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.UserAdministratorNotFound));

                    if (await _userAdministratorRepository.CheckByAsync(x => x.Email == model.Email && x._id != userAdministratorEntity._id))
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.EmailInUse));


                    userAdministratorEntity.SetIfDifferent(model, validOnly);

                    //if (string.IsNullOrEmpty(model.Password) == false)
                    //    userAdministratorEntity.Password = Utilities.GerarHashMd5(model.Password);

                    await _userAdministratorRepository.UpdateAsync(userAdministratorEntity).ConfigureAwait(false);

                }

                return Ok(Utilities.ReturnSuccess(string.IsNullOrEmpty(model.Id) ? "Registrado com sucesso" : "Atualizado com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// METODO DE LOGIN
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "email":"string",
        ///              "password":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Token")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Token([FromBody] LoginAdminViewModel model)
        {
            var claims = new List<Claim>();
            var claim = Util.SetRole(TypeProfile.Admin);
            var claimUserName = Request.GetUserName();
            claims.Add(claim);

            try
            {
                if (claimUserName != null)
                    claims.Add(claimUserName);

                model.TrimStringProperties();

                //if (string.IsNullOrEmpty(model.RefreshToken) == false)
                //    return TokenProviderMiddleware.RefreshToken(model.RefreshToken, false, claim);

                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var userAdministratorEntity = await _userAdministratorRepository.FindOneByAsync(x =>
                   x.Email == model.Email && x.Password == model.Password).ConfigureAwait(false);

                if (userAdministratorEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.UserAdministratorNotFound));

                if (userAdministratorEntity.DataBlocked != null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.UserAdministratorBlocked));


                if (claims.Count(x => x.Type == "UserName") == 0)
                    claims.Add(new Claim("UserName", userAdministratorEntity.Name));

                return Ok(Utilities.ReturnSuccess(data: TokenProviderMiddleware.GenerateToken(userAdministratorEntity._id.ToString(), false, claims.ToArray())));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// ESQUECI A SENHA
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///             POST
        ///                 {
        ///                  "email":"string"
        ///                 }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ForgotPassword([FromBody] LoginAdminViewModel model)
        {
            try
            {
                model?.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Email));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entity = await _userAdministratorRepository.FindOneByAsync(x => x.Email == model.Email).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileUnRegistred));

                var dataBody = Util.GetTemplateVariables();

                var newPassword = Utilities.RandomString(8);

                dataBody.Add("{{ title }}", "Lembrete de senha");
                dataBody.Add("{{ message }}", $"<p>Olá {entity.Name.GetFirstName()}</p>" +
                                            $"<p>Segue sua senha de acesso ao Moralar - Dashboard</p>" +
                                            $"<p><b>Login</b> : {entity.Login}</p>" +
                                            $"<p><b>Senha</b> :{newPassword}</p>");

                var body = _senderMailService.GerateBody("custom", dataBody);

                var unused = Task.Run(async () =>
               {
                   await _senderMailService.SendMessageEmailAsync($"Moralar-Dashboad", entity.Email, body, "Lembrete de senha").ConfigureAwait(false);

                   entity.Password = newPassword;
                   await _userAdministratorRepository.UpdateAsync(entity);

               }).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess("Verifique seu e-mail"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// REMOVER UM PERFIL DE ACESSO
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
        [HttpPost("Delete")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        
        public async Task<IActionResult> Delete([FromBody] UserAdministratorViewModel model)
        {
            try
            {
                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                if (string.IsNullOrEmpty(model.Id))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidIdentifier));

                await _userAdministratorRepository.DeleteOneAsync(model.Id).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess("Removido com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// Bloquear / Desbloquear 
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "targetId":"string"
        ///              "block":true
        ///             }
        /// </remarks>
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
                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var userAdministrator =
                    await _userAdministratorRepository.FindByIdAsync(model.TargetId).ConfigureAwait(false);

                if (userAdministrator == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.UserAdministratorNotFound));

                userAdministrator.DataBlocked = model.Block ? DateTimeOffset.Now.ToUnixTimeSeconds() : (long?)null;

                await _userAdministratorRepository.UpdateAsync(userAdministrator).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess(model.Block ? "Bloqueado com sucesso" : "Desbloqueado com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// LISTAGEM DE USUARIOS DE ACESSO AO ADMIN
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(UserAdministratorViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] int? blocked)
        {
            var response = new DtResult<UserAdministratorViewModel>();
            try
            {
                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = await _userAdministratorRepository.CountAsync(x => x.Created != null);

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<UserAdministrator>.Sort.Descending(sortColumn)
                    : Builders<UserAdministrator>.Sort.Ascending(sortColumn);

                var retorno = await _userAdministratorRepository
                    .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                    ? (int)await _userAdministratorRepository.CountSearchDataTableAsync(model.Search.Value, columns)
                    : totalRecords;

                var data = _mapper.Map<List<UserAdministratorViewModel>>(retorno);

                bool block = blocked != null && blocked == 1;
                response.Data = blocked == null ? data :  data.Where(x => x.Blocked == block).ToList();
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

    }
}