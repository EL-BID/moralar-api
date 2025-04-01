using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Entity
{
    public class PayerModel
    {
        //
        // Resumen:
        //     CPF ou CNPJ do Cliente
        [JsonProperty("cpf_cnpj")]
        [Display(Name = "Cpf Ou Cnpj")]
        public string CpfOrCnpj { get; set; }

        //
        // Resumen:
        //     Nome (utilizado como sacado em caso de pagamentos em boleto)
        [JsonProperty("name")]
        public string Name { get; set; }

        //
        // Resumen:
        //     Prefixo do Telefone (Ex: 11 para São Paulo)
        [JsonProperty("phone_prefix")]
        [Display(Name = "Prefixo do Telefone")]
        public string PhonePrefix { get; set; }

        //
        // Resumen:
        //     Telefone
        [JsonProperty("phone")]
        public string Phone { get; set; }

        //
        // Resumen:
        //     E-mail do Cliente
        [JsonProperty("email")]
        public string Email { get; set; }

        //
        // Resumen:
        //     Endereço do Cliente (utilizado em caso de pagamento em boleto)
        [JsonProperty("address")]
        public AddressModel Address { get; set; }

        public PayerModel()
        {
            Address = new AddressModel();
        }
    }
}
