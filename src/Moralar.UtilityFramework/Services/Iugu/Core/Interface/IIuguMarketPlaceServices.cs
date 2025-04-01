using Moralar.UtilityFramework.Services.Iugu.Core.Entity;
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Moralar.UtilityFramework.Services.Iugu.Core.Request;
using Moralar.UtilityFramework.Services.Iugu.Core.Response;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Interface
{
    public interface IIuguMarketPlaceServices
    {
        IuguTransferModel RepasseValores(string apiTokenSubConta, string accoutId, decimal valorDecimal, string apiTokenMaster = null, bool toWithdraw = true);

        IuguWithdrawalModel SolicitarSaque(string accoutId, decimal valorSaque, string apiToken = null);

        IuguAccountCreateResponseModel CriarSubConta(IuguAccountRequestModel model);

        IuguVerifyAccountModel VerificarSubConta(IuguAccountVerificationModel model, string userApiToken, string accoutId);

        SimpleResponseMessage AtualizarDadosBancariosSubConta(IuguUpdateDataBank model, string userApiToken);

        List<IuguBankVerificationResponse> VerificarAtualizacaoDadosBancariosSubConta(string userApiToken);

        IuguAccountCompleteModel GetInfoSubConta(string accoutId, string apiToken = null);

        IuguAccountCompleteModel ConfigurarSubConta(AccountConfigurationRequestMessage configurationRequest, string apiToken = null);

        IEnumerable<IuguAccountsModel> GetAccounts(IuguAccountFilterModel filter, string apiToken = null);

        Task<IuguTransferModel> RepasseValoresAsync(string apiTokenSubConta, string accoutId, decimal valorDecimal, string apiTokenMaster = null, bool toWithdraw = true);

        Task<IuguWithdrawalModel> SolicitarSaqueAsync(string accoutId, decimal valorSaque, string apiToken = null);

        Task<IuguAccountCreateResponseModel> CriarSubContaAsync(IuguAccountRequestModel model);

        Task<IuguVerifyAccountModel> VerificarSubContaAsync(IuguAccountVerificationModel model, string userApiToken, string accoutId);

        Task<SimpleResponseMessage> AtualizarDadosBancariosSubContaAsync(IuguUpdateDataBank model, string userApiToken);

        Task<List<IuguBankVerificationResponse>> VerificarAtualizacaoDadosBancariosSubContaAsync(string userApiToken);

        Task<IuguAccountCompleteModel> GetInfoSubContaAsync(string accoutId, string apiToken = null);

        Task<IuguAccountCompleteModel> ConfigurarSubContaAsync(AccountConfigurationRequestMessage configurationRequest, string apiToken = null);

        Task<IEnumerable<IuguAccountsModel>> GetAccountsAsync(IuguAccountFilterModel filter, string apiToken = null);

        //
        // Resumen:
        //     VERIFICAR OU ATUALIZAR DADOS BANCÁRIOS
        //
        // Parámetros:
        //   model:
        //
        //   liveKey:
        //
        //   userApiKey:
        //
        //   accountKey:
        //
        //   useGoogleVerificationAddress:
        //
        //   googleKey:
        Task<IuguBaseMarketPlace> SendVerifyOrUpdateDataBankAsync(DataBankViewModel model, bool newRegister, string liveKey = null, string userApiKey = null, string accountKey = null, long? lastVerification = null, string marketplaceName = null, bool useGoogleVerificationAddress = false, string googleKey = null);

        //
        // Resumen:
        //     VERIFICAR OU ATUALIZAR DADOS BANCÁRIOS
        //
        // Parámetros:
        //   model:
        //
        //   liveKey:
        //
        //   userApiKey:
        //
        //   accountKey:
        //
        //   useGoogleVerificationAddress:
        //
        //   googleKey:
        IuguBaseMarketPlace SendVerifyOrUpdateDataBank(DataBankViewModel model, bool newRegister, string liveKey = null, string userApiKey = null, string accountKey = null, long? lastVerification = null, string marketplaceName = null, bool useGoogleVerificationAddress = false, string googleKey = null);
    }
}
