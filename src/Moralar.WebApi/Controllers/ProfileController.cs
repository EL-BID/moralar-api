using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Domain.ViewModels.Profile;
using Moralar.Repository.Interface;
using Moralar.WebApi.Services;
using OfficeOpenXml;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;
using Moralar.Domain.Services;


namespace Moralar.WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class ProfileController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;
        private readonly ISenderMailService _senderMailService;
        private readonly IUtilService _utilService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(IWebHostEnvironment env, IMapper mapper, IProfileRepository profileRepository, ISenderMailService senderMailService, IUtilService utilService, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _mapper = mapper;
            _profileRepository = profileRepository;
            _senderMailService = senderMailService;
            _utilService = utilService;
            _httpContextAccessor = httpContextAccessor;
        }




        /// <summary>
        /// DETALHES DO USUÁRIO
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

                var entity = await _profileRepository.FindByIdAsync(id).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<ProfileViewModel>(entity)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        ///  METODO PARA BAIXAR ARQUIVO DE EXEMPLO PARA IMPORTAÇÃO
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ExampleFile")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(ProfileImportViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExampleFile()
        {
            try
            {
                var fileName = "example.xlsx";
                var path = UtilService.getFilePathFromServer("/Content/ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var fullPathFile = Path.Combine(path, fileName);
                var listViewModel = new List<ProfileImportViewModel>();

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
        /// IMPORTAÇÃO DO GESTOR
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Gestor/FileImport")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(ProfileImportViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [OnlyAdministrator]
        public async Task<IActionResult> ImportGestor([FromForm] IFormFile file)
        {
            try
            {

                if (file == null || file.Length <= 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FileNotFound));

                var extension = MimeTypeMap.GetExtension(file.ContentType).ToLower();

                if (Util.AcceptedFiles.Count(x => x == extension) == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FileNotAllowed));

                var listEntityViewModel = await Util.ReadAndValidationExcel<ProfileImportViewModel>(file);

                if (listEntityViewModel.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ZeroItems));

                /*PERMITE O CADASTRO DO TTS CASO EXISTA UM GESTOR COM MESMO CPF*/
                //var builder = Builders<Data.Entities.Profile>.Filter;
                //var conditions = new List<FilterDefinition<Data.Entities.Profile>>();

                //conditions.Add(builder.In(x => x.Cpf, listEntityViewModel.Select(x => x.Cpf.OnlyNumbers()).ToList()));

                //var exists = await _profileRepository.GetCollectionAsync().FindSync(builder.And(conditions)).ToListAsync();

                //if (exists.Count() > 0)
                //{
                //    var messageError = "O(s) CPF's _ estão em uso na plataforma";
                //    messageError = messageError.Replace("_", string.Join(",", exists.Select(x => x.Cpf).ToList()).TrimEnd(','));
                //    return BadRequest(Utilities.ReturnErro(messageError));
                //}

                var listEntity = _mapper.Map<List<Data.Entities.Profile>>(listEntityViewModel);


                for (int i = 0; i < listEntity.Count(); i++)
                {
                    listEntity[i].TypeProfile = TypeUserProfile.Gestor;
                    listEntity[i].Password = Utilities.RandomString(8);
                }
                const int limit = 250;
                var registred = 0;
                var index = 0;

                while (listEntity.Count() > registred)
                {
                    var itensToRegister = listEntity.Skip(limit * index).Take(limit).ToList();

                    if (itensToRegister.Count() > 0)
                        await _profileRepository.CreateAsync(itensToRegister);
                    registred += limit;
                    index++;
                }

                for (int i = 0; i < listEntity.Count(); i++)
                {
                    const string title = "Lembrete de senha";
                    var message = new StringBuilder();
                    message.AppendLine($"<p>Olá {listEntity[i].Name.GetFirstName()}</p>");
                    message.AppendLine($"<p>Segue sua senha de acesso ao Moralar</p>");
                    message.AppendLine($"<p><strong>Cpf</strong> : {listEntity[i].Cpf}<br/>");
                    message.AppendLine($"<strong>Senha</strong> :{listEntity[i].Password}</p>");

                    var dataBody = Util.GetTemplateVariables();
                    dataBody.Add("{{ title }}", title);
                    dataBody.Add("{{ message }}", message.ToString());

                    var body = _senderMailService.GerateBody("custom", dataBody);

                    var profileEmail = listEntity[i].Email;

                    var unused = Task.Run(async () =>
                    {
                        await _senderMailService.SendMessageEmailAsync("Moralar", profileEmail, body, title).ConfigureAwait(false);
                    });

                }

                return Ok(Utilities.ReturnSuccess($"Importação realizada com sucesso, total de {listEntity.Count()} gestor(es)"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// IMPORTAÇÃO DOS TTS
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("TTS/FileImport")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(ProfileImportViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ImportTTS([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length <= 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FileNotFound));

                var extension = MimeTypeMap.GetExtension(file.ContentType).ToLower();

                if (Util.AcceptedFiles.Count(x => x == extension) == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.FileNotAllowed));

                var listEntityViewModel = await Util.ReadAndValidationExcel<ProfileImportViewModel>(file);

                if (listEntityViewModel.Count() == 0)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ZeroItems));

                /*PERMITE O CADASTRO DO GESTOR CASO EXISTA UM TTS COM MESMO CPF*/
                //var builder = Builders<Data.Entities.Profile>.Filter;
                //var conditions = new List<FilterDefinition<Data.Entities.Profile>>();

                //conditions.Add(builder.In(x => x.Cpf, listEntityViewModel.Select(x => x.Cpf.OnlyNumbers()).ToList()));

                //var exists = await _profileRepository.GetCollectionAsync().FindSync(builder.And(conditions)).ToListAsync();

                //if (exists.Count() > 0)
                //{
                //    var messageError = "O(s) CPF's _ estão em uso na plataforma";
                //    messageError = messageError.Replace("_", string.Join(",", exists.Select(x => x.Cpf).ToList()).TrimEnd(','));
                //    return BadRequest(Utilities.ReturnErro(messageError));
                //}

                var listEntity = _mapper.Map<List<Data.Entities.Profile>>(listEntityViewModel);


                for (int i = 0; i < listEntity.Count(); i++)
                {
                    listEntity[i].TypeProfile = TypeUserProfile.TTS;
                    listEntity[i].Password = Utilities.RandomString(8);
                }
                const int limit = 250;
                var registred = 0;
                var index = 0;

                while (listEntity.Count() > registred)
                {
                    var itensToRegister = listEntity.Skip(limit * index).Take(limit).ToList();

                    if (itensToRegister.Count() > 0)
                        await _profileRepository.CreateAsync(itensToRegister);
                    registred += limit;
                    index++;
                }

                for (int i = 0; i < listEntity.Count(); i++)
                {
                    const string title = "Lembrete de senha";
                    var message = new StringBuilder();
                    message.AppendLine($"<p>Olá {listEntity[i].Name.GetFirstName()}</p>");
                    message.AppendLine($"<p>Segue sua senha de acesso ao Moralar</p>");
                    message.AppendLine($"<p><strong>Cpf</strong> : {listEntity[i].Cpf}<br/>");
                    message.AppendLine($"<strong>Senha</strong> :{listEntity[i].Password}</p>");

                    var dataBody = Util.GetTemplateVariables();
                    dataBody.Add("{{ title }}", title);
                    dataBody.Add("{{ message }}", message.ToString());

                    var body = _senderMailService.GerateBody("custom", dataBody);

                    var profileEmail = listEntity[i].Email;

                    var _ = Task.Run(async () =>
                    {
                        await _senderMailService.SendMessageEmailAsync("Moralar", profileEmail, body, title).ConfigureAwait(false);
                    });
                }

                return Ok(Utilities.ReturnSuccess($"Importação realizada com sucesso, total de {listEntity.Count()} Profissional TTS"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// INFORMAÇÕES  DO USUÁRIO
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

                var entity = await _profileRepository.FindByIdAsync(userId).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));

                if (entity.DataBlocked != null)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ReturnViewModel()
                    {
                        Erro = true,
                        Message = $"Usuário bloqueado,motivo {entity.Reason ?? "não informado"}"
                    });
                var vw = _mapper.Map<ProfileViewModel>(entity);
                //vw.Password = entity.Password;
                return Ok(Utilities.ReturnSuccess(data: vw));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// REMOVER PERFIL
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
        [OnlyAdministrator]
        public async Task<IActionResult> Delete([FromBody] BaseViewModel model)
        {
            try
            {
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                if (ObjectId.TryParse(model.Id, out var unused) == false)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                await _profileRepository.DeleteOneAsync(model.Id).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess("Removido com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// REMOVER PERFIL BY E-MAIL
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "email":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("DeleteByEmail")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [OnlyAdministrator]
        public async Task<IActionResult> DeleteByEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || email.ValidEmail() == false)
                    return BadRequest(Utilities.ReturnErro("Informe um e-mail valido"));

                await _profileRepository.DeleteAsync(x => x.Email == email.ToLower()).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess("Removido com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
        /// <summary>
        /// EDITAR PERFIL
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "name" : "string",
        ///              "jobPost" : "string",       
        ///              "email" : "string",
        ///              "phone" : "string",             
        ///              "id": "string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("UpdateProfile")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateViewModel model)
        {
            try
            {

                var validOnly = await Utilities.GetFieldsFromBodyAsync(Utilities.ConvertObjectToDictionary(model));

                if (string.IsNullOrEmpty(model.Id))
                    return BadRequest(Utilities.ReturnErro());

                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entity = await _profileRepository.FindByIdAsync(model.Id).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));

                entity.SetIfDifferent(model, validOnly);

                await _profileRepository.UpdateAsync(entity);

                return Ok(Utilities.ReturnSuccess("Alterada com sucesso."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// REGISTRAR USUÁRIO
        /// </summary>
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
        public async Task<IActionResult> Register([FromBody] ProfileRegisterViewModel model)
        {
            var claim = Util.SetRole(TypeProfile.Gestor);
            try
            {
                model.TrimStringProperties();
                var ignoreValidation = new List<string>();

                var isInvalidState = ModelState.ValidModelState(ignoreValidation.ToArray());

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                //if (model.TypeProvider != TypeProvider.Password)
                //{

                //    if (string.IsNullOrEmpty(model.ProviderId))
                //        return BadRequest(Utilities.ReturnErro(DefaultMessages.EmptyProviderId));
                //    var messageErro = "";
                //    if (await _profileRepository.CheckByAsync(x => x.ProviderId == model.ProviderId))
                //    {
                //        switch (model.TypeProvider)
                //        {
                //            case TypeProvider.Apple:
                //                messageErro = DefaultMessages.AppleIdInUse;
                //                break;
                //            default:
                //                messageErro = DefaultMessages.FacebookInUse;
                //                break;
                //        }
                //        return BadRequest(Utilities.ReturnErro(messageErro));

                //    }

                //    model.Login = model.Email;
                //}

                /*PERMITE O CADASTRO DE OUTROS PERFIS COM O MESMO CPF*/
                if (model.TypeProfile == TypeUserProfile.TTS)
                {
                    if (await _profileRepository.CheckByAsync(x => x.Cpf == model.Cpf && x.TypeProfile == TypeUserProfile.TTS).ConfigureAwait(false))
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.CpfInUse));
                }
                else
                {
                    if (await _profileRepository.CheckByAsync(x => x.Cpf == model.Cpf && x.TypeProfile == TypeUserProfile.Gestor).ConfigureAwait(false))
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.CpfInUse));
                }



                if (await _profileRepository.CheckByAsync(x => x.Email == model.Email).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.EmailInUse));

                var entity = _mapper.Map<Data.Entities.Profile>(model);

                entity.Password = Utilities.RandomInt(8);

                var entityId = await _profileRepository.CreateAsync(entity).ConfigureAwait(false);

                var dataBody = Util.GetTemplateVariables();
                dataBody.Add("{{ title }}", "Lembrete de senha");
                dataBody.Add("{{ message }}", $"<p>Olá {model.Name.GetFirstName()}</p>" +
                                            $"<p>Segue sua senha de acesso ao Moralar</p>" +
                                            //$"<p><b>Login</b> : {profile.Login}</p>" +
                                            $"<p><b>Senha</b> :{entity.Password}</p>"
                                            );

                var body = _senderMailService.GerateBody("custom", dataBody);

                var unused = Task.Run(async () =>
                {
                    await _senderMailService.SendMessageEmailAsync("Moralar", model.Email, body, "Lembrete de senha").ConfigureAwait(false);
                });
                //return Ok(Utilities.ReturnSuccess(data: TokenProviderMiddleware.GenerateToken(entityId, false, claim)));
                return Ok(Utilities.ReturnSuccess("Registrado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// LOGIN
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "login":"string", //optional
        ///              "password":"string", //optional
        ///              "providerId":"string", //optional
        ///              "typeProvider":0 //Enum (optional)
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
        public async Task<IActionResult> Token([FromBody] LoginViewModel model)
        {
            var claims = new List<Claim>();
            var claim = model.TypeUserProfile == TypeUserProfile.Gestor ? Util.SetRole(TypeProfile.Gestor) : Util.SetRole(TypeProfile.TTS);
            var claimUserName = Request.GetUserName();
            claims.Add(claim);

            try
            {
                if (claimUserName != null)
                    claims.Add(claimUserName);

                model.TrimStringProperties();


                if (string.IsNullOrEmpty(model.RefreshToken) == false)
                    return TokenProviderMiddleware.RefreshToken(model.RefreshToken, false, claims.ToArray());



                Data.Entities.Profile profileEntity;
                if (model.TypeProvider != TypeProvider.Password)
                {
                    profileEntity = await _profileRepository.FindOneByAsync(x => x.ProviderId == model.ProviderId)
                        .ConfigureAwait(false);

                    if (profileEntity == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound, new { IsRegister = true }));
                }
                else
                {
                    model.TrimStringProperties();
                    var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Login), nameof(model.Password));

                    if (isInvalidState != null)
                        return BadRequest(isInvalidState);

                    profileEntity = !string.IsNullOrEmpty(model.Cpf) ? await _profileRepository.FindOneByAsync(x => x.Cpf == model.Cpf && x.Password == model.Password && x.TypeProfile == model.TypeUserProfile).ConfigureAwait(false)
                            : await _profileRepository.FindOneByAsync(x => x.Email == model.Email && x.Password == model.Password && x.TypeProfile == model.TypeUserProfile).ConfigureAwait(false);


                    if (profileEntity == null)
                        return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidLogin));

                }

                if (claims.Count(x => x.Type == "UserName") == 0)
                    claims.Add(new Claim("UserName", profileEntity.Name));

                if (profileEntity.DataBlocked != null)
                    return BadRequest(Utilities.ReturnErro(string.Format(DefaultMessages.AccessBlockedWithReason, profileEntity.Reason.MapReason())));

                return Ok(Utilities.ReturnSuccess(data: TokenProviderMiddleware.GenerateToken(profileEntity._id.ToString(), false, claims.ToArray())));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// BLOQUEAR / DESBLOQUEAR USUÁRIO - ADMIN
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
        [HttpPost("TTS/BlockUnBlock")]
        [HttpPost("Gestor/BlockUnBlock")]
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

                var entity = await _profileRepository.FindByIdAsync(model.TargetId);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));

                entity.DataBlocked = model.Block ? DateTimeOffset.Now.ToUnixTimeSeconds() : (long?)null;
                entity.Reason = model.Block ? model.Reason : null;

                await _profileRepository.UpdateAsync(entity);

                return Ok(Utilities.ReturnSuccess(model.Block ? "Bloqueado com sucesso" : "Desbloqueado com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// ALTERAR PASSWORD
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "currentPassword":"string",
        ///              "newPassword":"string",
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            try
            {
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro());

                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var entity = await _profileRepository.FindByIdAsync(userId).ConfigureAwait(false);

                if (entity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));

                //if (entity.Password != model.CurrentPassword)
                //    return BadRequest(Utilities.ReturnErro(DefaultMessages.PasswordNoMatch));

                entity.LastPassword = entity.Password;
                entity.Password = model.NewPassword;

                _profileRepository.Update(entity);

                return Ok(Utilities.ReturnSuccess("Senha alterada com sucesso."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// METODO DE LISTAGEM COM DATATABLE - TODOS 
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Gestor/LoadData")]
        [HttpPost("TTS/LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(ProfileViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] TypeUserProfile typeProfile)
        {
            //string name = "sergio";
            var response = new DtResult<ProfileViewModel>();
            try
            {
                var builder = Builders<Data.Entities.Profile>.Filter;
                var conditions = new List<FilterDefinition<Data.Entities.Profile>>();

                conditions.Add(builder.Where(x => x.Created != null));
                conditions.Add(builder.Where(x => x.TypeProfile == typeProfile));
                //if (!string.IsNullOrEmpty(name))
                //    conditions.Add(builder.Where(x => x.Name.ToUpper().StartsWith(name.ToUpper())));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = !string.IsNullOrEmpty(model.SortOrder) ? model.SortOrder.UppercaseFirst() : model.Columns.FirstOrDefault(x => x.Orderable)?.Name ?? model.Columns.FirstOrDefault()?.Name;
                var totalRecords = (int)await _profileRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));
                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                    ? Builders<Data.Entities.Profile>.Sort.Descending(sortColumn)
                    : Builders<Data.Entities.Profile>.Sort.Ascending(sortColumn);

                string search = (model.Search.Value != null) ? model.Search.Value.Trim() : null;
                if (search != null)
                {
                    search = search.Replace(".", "").Replace("-", "");
                }

                var retorno = await _profileRepository
                    .LoadDataTableAsync(search, sortBy, model.Start, model.Length, conditions, columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(search)
                    ? (int)await _profileRepository.CountSearchDataTableAsync(search, conditions, columns)
                    : totalRecords;

                response.Data = _mapper.Map<List<ProfileViewModel>>(retorno);
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
        /// REGISTRAR E REMOVER DEVICE ID ONESIGNAL OU FCM | CHAMAR APOS LOGIN E LOGOUT
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "deviceId":"string",
        ///              "isRegister":true  // true => registrar  | false => remover 
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("RegisterUnRegisterDeviceId")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult RegisterUnRegisterDeviceId([FromBody] PushViewModel model)
        {
            try
            {
                var userId = Request.GetUserId();

                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));

                Task.Run(() =>
               {
                   if (string.IsNullOrEmpty(model.DeviceId) == false)
                   {
                       if (model.IsRegister)
                       {
                           _profileRepository.UpdateMultiple(Query<Data.Entities.Profile>.Where(x => x._id == ObjectId.Parse(userId)),
                           new UpdateBuilder<Data.Entities.Profile>().AddToSet(x => x.DeviceId, model.DeviceId), UpdateFlags.None);
                       }
                       else
                       {
                           _profileRepository.UpdateMultiple(Query<Data.Entities.Profile>.Where(x => x._id == ObjectId.Parse(userId)),
                           new UpdateBuilder<Data.Entities.Profile>().Pull(x => x.DeviceId, model.DeviceId), UpdateFlags.None);
                       }
                   }
               });

                return Ok(Utilities.ReturnSuccess());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// UPDATE DE USUÁRIO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///         POST
        ///             {
        ///              "fullName":"string",
        ///              "email":"string",
        ///              "photo":"string", //filename
        ///              "phone":"string", //(##) #####-####
        ///              "login":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Update")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update([FromBody] ProfileRegisterViewModel model)
        {
            try
            {

                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelState();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var _id = ObjectId.Parse(userId);

                if (await _profileRepository.CheckByAsync(x => x.Email == model.Email && x._id != _id))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.EmailInUse));


                //if (await _profileRepository.CheckByAsync(x => x.Login == model.Login && x._id != _id))
                //    return BadRequest(Utilities.ReturnErro(DefaultMessages.LoginInUse));

                var profileEntity = await _profileRepository.FindByIdAsync(userId).ConfigureAwait(false);

                profileEntity.Email = model.Email;
                profileEntity.Name = model.Name;
                profileEntity.Phone = model.Phone?.OnlyNumbers();
                profileEntity.JobPost = model.JobPost;
                //profileEntity.Login = model.Login;
                //profileEntity.Photo = model.Photo.RemovePathImage().SetPhotoProfile(profileEntity.ProviderId);

                profileEntity = await _profileRepository.UpdateAsync(profileEntity).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess(data: _mapper.Map<ProfileViewModel>(profileEntity)));
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
        ///         POST
        ///             {
        ///              "cpf":"string"
        ///             }
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
        public async Task<IActionResult> ForgotPassword([FromBody] LoginViewModel model)
        {
            try
            {
                model?.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Cpf));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                var profile = await _profileRepository.FindOneByAsync(x => x.Cpf == model.Cpf).ConfigureAwait(false);

                if (profile == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileUnRegistred));

                var dataBody = Util.GetTemplateVariables();

                var newPassword = Utilities.RandomInt(8);

                dataBody.Add("{{ title }}", "Lembrete de senha");
                dataBody.Add("{{ message }}", $"<p>Olá {profile.Name.GetFirstName()}</p>" +
                                            $"<p>Segue sua senha de acesso ao Moralar</p>" +
                                            $"<p><b>CPF</b> : {profile.Cpf}</p>" +
                                            $"<p><b>Senha</b> :{newPassword}</p>");

                var body = _senderMailService.GerateBody("custom", dataBody);

                var unused = Task.Run(async () =>
                {
                    await _senderMailService.SendMessageEmailAsync("Moralar", profile.Email, body, "Lembrete de senha").ConfigureAwait(false);

                    profile.Password = newPassword;
                    await _profileRepository.UpdateAsync(profile);

                });

                return Ok(Utilities.ReturnSuccess("Verifique seu e-mail"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// VERIFICAR SE E-MAIL ESTÁ EM USO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///             POST
        ///             {
        ///                 "email":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("CheckEmail")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckEmail([FromBody] ValidationViewModel model)
        {
            try
            {
                model?.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Email));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                if (await _profileRepository.CheckByAsync(x => x.Email == model.Email).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.EmailInUse));

                return Ok(Utilities.ReturnSuccess("Disponível"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// VERIFICAR SE E-MAIL ESTÁ EM USO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///             POST
        ///             {
        ///                 "login":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("CheckLogin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckLogin([FromBody] ValidationViewModel model)
        {
            try
            {
                model?.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Login));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                if (await _profileRepository.CheckByAsync(x => x.Cpf == model.Cpf).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.LoginInUse));

                return Ok(Utilities.ReturnSuccess("Disponível"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// VERIFICAR SE CPF ESTÁ EM USO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///             POST
        ///             {
        ///                 "cpf":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("CheckCpf")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckCpf([FromBody] ValidationViewModel model)
        {
            try
            {
                model?.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields(nameof(model.Cpf));

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                if (await _profileRepository.CheckByAsync(x => x.Cpf == model.Cpf).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CpfInUse));

                return Ok(Utilities.ReturnSuccess("Disponível"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        /// <summary>
        /// VERIFICAR SE CAMPOS ESTÁ EM USO
        /// </summary>
        /// <remarks>
        /// OBJ DE ENVIO
        /// 
        ///             POST
        ///             {
        ///                 "cpf":"string",
        ///                 "login":"string",
        ///                 "email":"string"
        ///             }
        /// </remarks>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("CheckAll")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckAll([FromBody] ValidationViewModel model)
        {
            try
            {
                model.TrimStringProperties();
                var isInvalidState = ModelState.ValidModelStateOnlyFields();

                if (isInvalidState != null)
                    return BadRequest(isInvalidState);

                if (string.IsNullOrEmpty(model.Cpf) == false && await _profileRepository.CheckByAsync(x => x.Cpf == model.Cpf).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.CpfInUse));

                //if (string.IsNullOrEmpty(model.Login) == false && await _profileRepository.CheckByAsync(x => x.Cpf == model.cp).ConfigureAwait(false))
                //    return BadRequest(Utilities.ReturnErro(DefaultMessages.LoginInUse));

                if (string.IsNullOrEmpty(model.Email) == false && await _profileRepository.CheckByAsync(x => x.Email == model.Email).ConfigureAwait(false))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.EmailInUse));


                return Ok(Utilities.ReturnSuccess("Disponível"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }


        // METODO PARA UPDATE / REGISTRO DE DADOS BANCÁRIOS
        // /// <summary>
        // /// ATUALIZAR DADOS BANCÁRIOS |  Bank = enviar CODIGO DO BANCO | EX = 341
        // /// </summary>
        // /// <response code="200">Returns success</response>
        // /// <response code="400">Custom Error</response>
        // /// <response code="401">Unauthorize Error</response>
        // /// <response code="500">Exception Error</response>
        // /// <returns></returns>
        // [HttpPost("UpdateDataBank")]
        // [Produces("application/json")]
        // [ProducesResponseType(typeof(ReturnViewModel), 200)]
        // [ProducesResponseType(400)]
        // [ProducesResponseType(401)]
        // [ProducesResponseType(500)]
        // public async Task<IActionResult> UpdateDataBank([FromBody] Domain.ViewModels.DataBankViewModel model)
        // {
        //     try
        //     {
        //         var userId = Request.GetUserId();

        //         if (string.IsNullOrEmpty(userId))
        //             return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredencials));

        //         model.TrimStringProperties();
        //         var isInvalidState = ModelState.ValidModelState();

        //         if (isInvalidState != null)
        //             return BadRequest(isInvalidState);

        //         if (string.IsNullOrEmpty(model.AccoutableCpf) == false && model.AccoutableCpf.ValidCpf() == false)
        //             return BadRequest(Utilities.ReturnErro(DefaultMessages.CpfInvalid));

        //         if (string.IsNullOrEmpty(model.Cpnj) == false && model.Cpnj.ValidCnpj() == false)
        //             return BadRequest(Utilities.ReturnErro(DefaultMessages.CnpjInvalid));

        //         var profileLoad = await _profileRepository.FindByIdAsync(userId).ConfigureAwait(false);

        //         if (profileLoad == null)
        //             return BadRequest(Utilities.ReturnErro(DefaultMessages.ProfileNotFound));


        //         var iuguResponse = await _iuguMarketPlaceServices.SendVerifyOrUpdateDataBankAsync(new UtilityFramework.Services.Iugu.Core.Models.DataBankViewModel()
        //         {
        //             SocialName = profileLoad.FullName,
        //             PhysicalProducts = false,
        //             AccountableName = profileLoad.FullName,
        //             AccountType = model.TypeAccount,
        //             AccountableCpf = model.AccoutableCpf,
        //             BankAgency = model.BankAgency,
        //             BankAccount = model.BankAccount,
        //             Bank = model.Bank,
        //             FantasyName = profileLoad.FullName,
        //             Cpf = profileLoad.Cpf,
        //             Address = profileLoad.StreetAddress,
        //             LastRequestVerification = profileLoad.LastRequestVerification,
        //             PersonType = model.PersonType,
        //         }, string.IsNullOrEmpty(profileLoad.AccountKey),
        //         profileLoad.LiveKey,
        //         profileLoad.UserApiKey,
        //         profileLoad.AccountKey,
        //         profileLoad.LastRequestVerification,
        //         profileLoad.FullName).ConfigureAwait(false);

        //         if (iuguResponse.IsNewRegister && string.IsNullOrEmpty(iuguResponse.AccountKey) == false)
        //         {
        //             profileLoad.AccountKey = iuguResponse.AccountKey;
        //             profileLoad.LiveKey = iuguResponse.LiveKey;
        //             profileLoad.TestKey = iuguResponse.TestKey;
        //             profileLoad.UserApiKey = iuguResponse.UserApiKey;

        //             profileLoad = _profileRepository.Update(profileLoad);
        //         }

        //         if (iuguResponse.HasError)
        //             return BadRequest(Utilities.ReturnErro(iuguResponse.MessageError, errors: iuguResponse.Error));

        //         profileLoad.HasDataBank = true;
        //         profileLoad.LastRequestVerification = iuguResponse.LastRequestVerification;
        //         profileLoad.LastConfirmDataBank = null;
        //         profileLoad.TypeAccount = model.TypeAccount;
        //         profileLoad.Bank = model.Bank;
        //         profileLoad.BankAccount = iuguResponse.BankAccount;
        //         profileLoad.BankAgency = iuguResponse.BankAgency;
        //         profileLoad.AccoutableCpf = model.AccoutableCpf;
        //         profileLoad.AccoutableName = model.AccoutableName;
        //         profileLoad.PersonType = model.PersonType;

        //         await _profileRepository.UpdateAsync(profileLoad).ConfigureAwait(false);

        //         return Ok(Utilities.ReturnSuccess(iuguResponse.CustomMessage ?? "Dados atualizados com sucesso"));
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.ReturnErro());
        //     }
        // }
    }
}