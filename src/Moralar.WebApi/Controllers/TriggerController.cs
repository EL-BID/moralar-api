using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moralar.Data.Entities;
using Moralar.Domain;
using Moralar.Domain.Services.Interface;
using Moralar.Repository.Interface;
using Moralar.UtilityFramework.Services.Iugu.Core.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;
using Moralar.UtilityFramework.Services.Core.Interface;

// ReSharper disable NotAccessedField.Local
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable 1998

namespace Moralar.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    public class TriggerController : Controller
    {

        private readonly ISenderMailService _senderMailService;
        private readonly ISenderNotificationService _senderNotificationService;
        private readonly IProfileRepository _profileRepository;
        private readonly IIuguChargeServices _iuguChargeServices;

        private readonly string _projectName;

        public TriggerController(ISenderMailService senderMailService, ISenderNotificationService senderNotificationService, IProfileRepository profileRepository, IIuguChargeServices iuguChargeServices)
        {
            _senderMailService = senderMailService;
            _senderNotificationService = senderNotificationService;
            _profileRepository = profileRepository;
            _iuguChargeServices = iuguChargeServices;
            _projectName = Assembly.GetEntryAssembly().GetName().Name?.Split('.')[0];

        }

        /// <summary>
        /// GATILHOS DA IUGU
        /// </summary>
        /// <response code="200">Returns success</response>
        /// <response code="400">Custom Error</response>
        /// <response code="401">Unauthorize Error</response>
        /// <response code="500">Exception Error</response>
        /// <returns></returns>
        //[AllowAnonymous]
        //[HttpPost("ChangeStatusGatway")]
        //[Produces("application/json")]
        //[ProducesResponseType(typeof(ReturnViewModel), 200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[ProducesResponseType(500)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<IActionResult> ChangeStatusGatway([FromForm] IuguTriggerModel model)
        //{
        //    try
        //    {
        //        model.TrimStringProperties();
        //        var isInvalidState = ModelState.ValidModelState();

        //        if (isInvalidState != null)
        //            return BadRequest(isInvalidState);

        //        model.SetAllProperties(Request.Form);

        //        var registerLog = false;
        //        Profile profile = null;

        //        var suportEmail = Utilities.GetConfigurationRoot().GetSection("suportEmail").Get<List<string>>();
        //        var clientIuguSettigns = Utilities.GetConfigurationRoot().GetSection("IUGU:Client").Get<MegaClientIuguViewModel>();


        //        switch (model.Event)
        //        {
        //            case TriggerEvents.InvoiceReleased:


        //                var invoideLoad = _iuguChargeServices.GetFatura(model.Data.Id);

        //                if (invoideLoad == null)
        //                    _senderMailService.SendMessageEmail(_projectName, suportEmail, $"Fatura não encontrada id da fatura = {model.Data.Id} DATA:{JsonConvert.SerializeObject(model).JsonPrettify()}", "Fatura não encontradda");

        //                const decimal taxApp = 5M;
        //                const decimal advanceTax = 2.5M;
        //                const decimal iuguTaxPercent = 2.51M;
        //                decimal totalValue = 0; // VALOR TOTAL DA TRANSAÇÃO

        //                // VALOR LIQUIDO - (LIBERADO NA IUGU)
        //                var netValue = decimal.Parse(invoideLoad.FinancialReturnDates[0].Amount, NumberStyles.Currency);

        //                var cardTax = (iuguTaxPercent / 100M) * totalValue;
        //                // VALOR REFERENTE A ANTECIPAÇÃO
        //                var valueAdvance = invoideLoad.FinancialReturnDates[0].Advanced == true ? (advanceTax / 100M) * totalValue : 0;
        //                // TAXAS COBRADAS PELA IUGU
        //                var iuguTax = cardTax + valueAdvance;

        //                // TAXAS RETIDAS PARA O CARRO66
        //                var totalTaxApp = (taxApp / 100M) * totalValue;

        //                // TAXA MEGALEIOS   
        //                var totalTaxaMegaleios = ((decimal)clientIuguSettigns.Tax / 100M) * totalValue;

        //                var allTax = (totalTaxApp + totalTaxaMegaleios + iuguTax);

        //                var priceRepasse = (totalValue - allTax).NotAround();


        //                break;
        //            case TriggerEvents.ReferralsVerification:

        //                var body = new StringBuilder();

        //                profile = await _profileRepository.FindOneByAsync(x => x.AccountKey == model.Data.AccountId).ConfigureAwait(false);

        //                // caso encontre o dono do estabelecimento
        //                if (profile != null)
        //                {

        //                    var status = model.Data.Status.ToLower();

        //                    body.AppendLine($"<p>Olá {profile.Name.GetFirstName()}</p>");

        //                    var dataBody = Util.GetTemplateVariables();

        //                    dataBody.Add("{{ title }}", "Verificação de dados bancários");

        //                    // if (Equals(status, "accepted"))
        //                    // {
        //                    //     profile.HasDataBank = true;
        //                    //     profile.LastConfirmDataBank = DateTimeOffset.Now.ToUnixTimeSeconds();
        //                    //     profile.LastRequestVerification = null;

        //                    //     body.AppendLine("<p>Informamos que seus dados bancários foram válidados com sucesso, você já pode receber suas transações com cartão de credito</p>");
        //                    // }
        //                    // else
        //                    // {
        //                    //     profile.LastConfirmDataBank = null;
        //                    //     profile.LastRequestVerification = null;

        //                    //     body.AppendLine("<p>Informamos que seus dados bancários encontram-se inválidos, por favor verifique os dados informados e atualize os mesmos</p>");
        //                    // }
        //                    dataBody.Add("{{ message }}", body.ToString());
        //                    var customBody = _senderMailService.GerateBody("custom", dataBody);

        //                    await _senderMailService.SendMessageEmailAsync(Startup.ApplicationName, new List<string>() { profile.Email }, body.ToString(), "Verificação de dados bancários", ccoEmails: suportEmail);

        //                    await _profileRepository.UpdateAsync(profile);
        //                }
        //                break;
        //        }

        //        if (registerLog)
        //        {
        //            var unused = Task.Run(() =>
        //           {
        //               var json = JsonConvert.SerializeObject(model, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }).JsonPrettify();

        //               _senderMailService.SendMessageEmail("Gatilho", suportEmail, json,
        //                   $"Gatilho {_projectName} - {model.Event}");
        //           });
        //        }
        //        return Ok(Utilities.ReturnSuccess());
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.ReturnErro());
        //    }
        //}
    }
}