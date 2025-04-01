
namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguWithdrawalModel : IuguBaseErrors
    {
        public string Agencia { get; set; }

        public string Banco { get; set; }

        public string Conta { get; set; }

        public string AccountId { get; set; }

        public string WithdrawalId { get; set; }

        public string Valor { get; set; }

        public string Type { get; set; }
    }
}
