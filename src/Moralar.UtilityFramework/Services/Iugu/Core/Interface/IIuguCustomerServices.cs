

using Moralar.UtilityFramework.Services.Iugu.Core.Models;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Interface
{
    public interface IIuguCustomerServices
    {
        IuguCustomer SaveClient(IuguCustomerCreated customer);

        IuguCustomer UpdateClient(IuguCustomerCreated customer);

        void DeleteClient(string id);

        IuguCustomer SetDefaultCartao(string clientId, string cardId, string cpfCnpj);

        IuguCustomer SetDefaultCartao(string clientId, string cardId);

        Task<IuguCustomer> SaveClientAsync(IuguCustomerCreated customer);

        Task<IuguCustomer> UpdateClientAsync(IuguCustomerCreated customer);

        Task DeleteClientAsync(string id);

        Task<IuguCustomer> SetDefaultCartaoAsync(string clientId, string cardId, string cpfCnpj);

        Task<IuguCustomer> SetDefaultCartaoAsync(string clientId, string cardId);
    }
}
