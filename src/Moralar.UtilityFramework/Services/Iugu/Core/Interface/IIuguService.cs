
using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Moralar.UtilityFramework.Services.Iugu.Core.Response;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Interface
{
    public interface IIuguService
    {
        //
        // Resumen:
        //     ENVIAR VERIFICAÇÃO DE DADOS BANCARIOS
        //
        // Parámetros:
        //   model:
        //
        //   userApiKey:
        //
        //   accountKey:
        //
        //   useGoogleVerificationAddress:
        //
        //   googleKey:
        IuguVerifyAccountModel SendRequestVerification(DataBankViewModel model, string userApiKey, string accountKey, bool useGoogleVerificationAddress = false, string googleKey = null);

        //
        // Resumen:
        //     ATUALIZAR DADOS BANCARIOS
        //
        // Parámetros:
        //   model:
        //
        //   liveKey:
        SimpleResponseMessage UpdateDataBank(DataBankViewModel model, string liveKey);

        //
        // Resumen:
        //     ENVIAR VERIFICAÇÃO DE DADOS BANCARIOS
        //
        // Parámetros:
        //   model:
        //
        //   userApiKey:
        //
        //   accountKey:
        //
        //   useGoogleVerificationAddress:
        //
        //   googleKey:
        Task<IuguVerifyAccountModel> SendRequestVerificationAsync(DataBankViewModel model, string userApiKey, string accountKey, bool useGoogleVerificationAddress = false, string googleKey = null);

        //
        // Resumen:
        //     ATUALIZAR DADOS BANCARIOS
        //
        // Parámetros:
        //   model:
        //
        //   liveKey:
        Task<SimpleResponseMessage> UpdateDataBankAsync(DataBankViewModel model, string liveKey);
    }

}
