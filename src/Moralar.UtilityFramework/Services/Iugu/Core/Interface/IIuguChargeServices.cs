using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Moralar.UtilityFramework.Services.Iugu.Core.Request;
using Moralar.UtilityFramework.Services.Iugu.Core.Response;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Interface
{
    public interface IIuguChargeServices
    {
        IuguInvoiceResponseMessage GerarFatura(InvoiceRequestMessage model, string apiToken = null);

        IuguInvoiceResponseMessage CancelarFatura(string invoiceId, string apiToken = null);

        IuguChargeResponse TransacaoCrediCard(IuguChargeRequest model, string clienteId, string idCartao = null, string token = null, string apiToken = null);

        IuguInvoiceResponseMessage GetFatura(string invoiceId, string apiToken = null);

        IuguChargeResponse EstornarFatura(string invoiceId, int? refundCents = null, string apiToken = null);

        SubscriptionResponseMessage GerarAssinatura(SubscriptionRequestMessage model, string apiToken = null);

        SubscriptionResponseMessage BuscarAssinatura(string subscriptionId, string apiToken = null);

        SubscriptionResponseMessage RemoverAssinatura(string subscriptionId, string apiToken = null);

        SubscriptionResponseMessage AlterarPlanoDaAssinatura(string subscriptionId, string planIdentifier, string apiToken = null);

        SubscriptionResponseMessage AtivarAsinatura(string signatureId, string apiToken = null);

        SubscriptionResponseMessage SuspenderAssinatura(string signatureId, string apiToken = null);

        IuguPlanModel CriarPlano(IuguPlanModel model, string apiToken = null);

        IuguPlanModel UpdatePlano(string planId, IuguPlanModel model, string apiToken = null);

        void RemoverPlano(string planId, string apiToken = null);

        ListIuguAdvanceResponse GetRecebiveis(int? skip = null, int? limit = null, string apiToken = null);

        Task<IuguChargeResponse> TransacaoCrediCardAsync(IuguChargeRequest model, string clienteId, string idCartao = null, string token = null, string apiToken = null);

        Task<IuguInvoiceResponseMessage> GetFaturaAsync(string invoiceId, string apiToken = null);

        Task<IuguChargeResponse> EstornarFaturaAsync(string invoiceId, int? refundCents = null, string apiToken = null);

        Task<SubscriptionResponseMessage> GerarAssinaturaAsync(SubscriptionRequestMessage model, string apiToken = null);

        Task<SubscriptionResponseMessage> BuscarAssinaturaAsync(string subscriptionId, string apiToken = null);

        Task<SubscriptionResponseMessage> RemoverAssinaturaAsync(string subscriptionId, string apiToken = null);

        Task<SubscriptionResponseMessage> AlterarPlanoDaAssinaturaAsync(string subscriptionId, string planIdentifier, string apiToken = null);

        Task<SubscriptionResponseMessage> AtivarAsinaturaAsync(string signatureId, string apiToken = null);

        Task<SubscriptionResponseMessage> SuspenderAssinaturaAsync(string signatureId, string apiToken = null);

        Task<IuguPlanModel> CriarPlanoAsync(IuguPlanModel model, string apiToken = null);

        Task<IuguPlanModel> UpdatePlanoAsync(string planId, IuguPlanModel model, string apiToken = null);

        Task RemoverPlanoAsync(string planId, string apiToken = null);

        Task<ListIuguAdvanceResponse> GetRecebiveisAsync(int? skip = null, int? limit = null, string apiToken = null);

        IuguAdvanceSimulationResponse SimularAntecipacao(IuguAdvanceRequest model, string apiToken = null);

        Task<IuguAdvanceSimulationResponse> SimularAntecipacaoAsync(IuguAdvanceRequest model, string apiToken = null);

        IuguAdvanceSimulationResponse SolicitarAntecipacao(IuguAdvanceRequest model, string apiToken = null);

        Task<IuguAdvanceSimulationResponse> SolicitarAntecipacaoAsync(IuguAdvanceRequest model, string apiToken = null);

        Task<IuguInvoiceResponseMessage> CancelarFaturaAsync(string invoiceId, string apiToken = null);

        Task<IuguInvoiceResponseMessage> GerarFaturaAsync(InvoiceRequestMessage model, string apiToken = null);

        List<WebHookViewModel> GetWebhookLog(string invoiceId, string apiToken = null);

        Task<List<WebHookViewModel>> GetWebhookLogAsync(string invoiceId, string apiToken = null);

        WebHookViewModel ResendWebhook(string webhookId, string apiToken = null);

        Task<WebHookViewModel> ResendWebhookAsync(string webhookId, string apiToken = null);
    }
}