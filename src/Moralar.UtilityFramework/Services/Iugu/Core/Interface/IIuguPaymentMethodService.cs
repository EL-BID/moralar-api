using Moralar.UtilityFramework.Services.Iugu.Core.Models;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Interface
{
    public interface IIuguPaymentMethodService
    {
        IuguPaymentMethodToken SaveCreditCard(IuguPaymentMethodToken model);

        IEnumerable<IuguCreditCard> ListarCredCards(string clientId);

        IuguCreditCard LinkCreditCardClient(IuguCustomerPaymentMethod model, string clienteId);

        IuguCreditCard BuscarCredCards(string clientId, string cardId);

        IuguCreditCard RemoverCredCard(string clientId, string cardId);

        Task<IuguPaymentMethodToken> SaveCreditCardAsync(IuguPaymentMethodToken model);

        Task<IEnumerable<IuguCreditCard>> ListarCredCardsAsync(string clientId);

        Task<IuguCreditCard> LinkCreditCardClientAsync(IuguCustomerPaymentMethod model, string clienteId);

        Task<IuguCreditCard> BuscarCredCardsAsync(string clientId, string cardId);

        Task<IuguCreditCard> RemoverCredCardAsync(string clientId, string cardId);
    }
}
