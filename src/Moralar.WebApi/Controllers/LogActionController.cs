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
using Moralar.Data.Enum;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.ViewModels;
using Moralar.Repository.Interface;
using Moralar.WebApi.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    public class LogActionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogActionRepository _logActionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogActionController(IMapper mapper, ILogActionRepository logActionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _logActionRepository = logActionRepository;
            _httpContextAccessor = httpContextAccessor;
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
        [OnlyAdministrator]
        public async Task<IActionResult> Export()
        {
            try
            {
                var allData = await _logActionRepository.FindAllAsync().ConfigureAwait(false);
                var fileName = $"logaction.xlsx";
                var path = UtilService.getFilePathFromServer("/Content/ExportFiles");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
                var fullPathFile = Path.Combine(path, fileName);
                var listViewModel = _mapper.Map<List<LogActionExportViewModel>>(allData);
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
        /// METODO DE DETALHES DO ITEM
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpGet("Detail/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(LogActionViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [OnlyAdministrator]
        public async Task<IActionResult> Detail([FromRoute] string id)
        {
            try
            {
                var userId = Request.GetUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidCredentials));

                var logActionEntity = await _logActionRepository.FindByIdAsync(id).ConfigureAwait(false);
                if (logActionEntity == null)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.LogActionNotFound));

                var entityViewModel = _mapper.Map<LogActionViewModel>(logActionEntity);
                return Ok(Utilities.ReturnSuccess(data: entityViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }

        /// <summary>
        /// METODO DE LISTAGEM COM DATATABLE
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("LoadData")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(typeof(LogActionViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [OnlyAdministrator]
        public async Task<IActionResult> LoadData([FromForm] DtParameters model, [FromForm] TypeAction? typeAction, [FromForm] LocalAction? localAction, [FromForm] long? startDate, [FromForm] long? endDate, [FromForm] TypeResposible? typeResponsible)
        {
            var response = new DtResult<LogActionViewModel>();
            try
            {
                var builder = Builders<LogAction>.Filter;
                var conditions = new List<FilterDefinition<LogAction>>();

                conditions.Add(builder.Where(x => x.Disabled == null));

                if (localAction != null)
                    conditions.Add(builder.Where(x => x.LocalAction == localAction));

                if (typeAction != null)
                    conditions.Add(builder.Where(x => x.TypeAction == typeAction));

                if (typeResponsible != null)
                    conditions.Add(builder.Where(x => x.TypeResposible == typeResponsible));


                if (startDate != null && startDate > 0)
                    conditions.Add(builder.Where(x => x.Created >= startDate));

                if (endDate != null && endDate > 0 && (startDate == null || startDate < endDate))
                    conditions.Add(builder.Where(x => x.Created <= endDate));

                var columns = model.Columns.Where(x => x.Searchable && !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray();

                var sortColumn = model.SortOrder;
                var totalRecords = (int)await _logActionRepository.GetCollectionAsync().CountDocumentsAsync(builder.And(conditions));

                var sortBy = model.Order[0].Dir.ToString().ToUpper().Equals("DESC")
                   ? Builders<LogAction>.Sort.Descending(sortColumn)
                   : Builders<LogAction>.Sort.Ascending(sortColumn);

                var retorno = await _logActionRepository
                 .LoadDataTableAsync(model.Search.Value, sortBy, model.Start, model.Length, conditions, columns);

                var totalrecordsFiltered = !string.IsNullOrEmpty(model.Search.Value)
                   ? (int)await _logActionRepository.CountSearchDataTableAsync(model.Search.Value, conditions, columns)
                   : totalRecords;

                response.Data = _mapper.Map<List<LogActionViewModel>>(retorno);
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
        /// METODO DE REMOVER ITEM
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        [HttpPost("Delete/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReturnViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [OnlyAdministrator]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                if (ObjectId.TryParse(id, out var unused) == false)
                    return BadRequest(Utilities.ReturnErro(DefaultMessages.InvalidIdentifier));

                await _logActionRepository.DeleteOneAsync(id).ConfigureAwait(false);

                return Ok(Utilities.ReturnSuccess(DefaultMessages.Deleted));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ReturnErro());
            }
        }
    }
}