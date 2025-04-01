
using Moralar.UtilityFramework.Services.Iugu.Core.Models;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class IuguBaseMarketPlace : IuguBaseErrors
    {
        public string AccoutableName { get; set; }

        public string AccoutableCpf { get; set; }

        public string BankAccount { get; set; }

        public string BankAgency { get; set; }

        public string Bank { get; set; }

        public string AccountType { get; set; }

        public string PersonType { get; set; }

        public string Cpnj { get; set; }

        public string AccountKey { get; set; }

        public string LiveKey { get; set; }

        public string TestKey { get; set; }

        public string UserApiKey { get; set; }

        public bool HasDataBank { get; set; }

        public long? LastRequestVerification { get; set; }

        public long? LastConfirmDataBank { get; set; }

        public bool IsNewRegister { get; set; }

        //
        // Resumen:
        //     INFORMA SE FOI UMA ATUALIZAÇÃO DE DADOS BANCARIOS
        public long? UpdateDataBank { get; set; }

        //
        // Resumen:
        //     INFORMA SE ESTA EM VERIFICAÇÃO
        public long? InVerification { get; set; }

        public string Cnpj { get; set; }

        public string CustomMessage { get; set; }
    }

}
