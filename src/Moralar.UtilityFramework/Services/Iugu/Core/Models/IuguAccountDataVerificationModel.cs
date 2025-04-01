
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class IuguAccountDataVerificationModel : IuguBaseErrors
    {
        //
        // Resumen:
        //     Validação automática
        [Display(Name = "Validação automática")]
        [JsonProperty("automatic_validation")]
        public bool AutomaticValidation { get; set; }

        //
        // Resumen:
        //     Valor máximo da venda
        [Display(Name = "Valor máximo da venda")]
        [JsonProperty("price_range")]
        public string PriceRange { get; set; }

        //
        // Resumen:
        //     Produtos físicos
        [Display(Name = "Produtos físicos")]
        [JsonProperty("physical_products")]
        public bool PhysicalProducts { get; set; }

        //
        // Resumen:
        //     RAMO DA EMPRESA
        [Display(Name = "Ramo da empresa")]
        [JsonProperty("business_type")]
        public string BusinessType { get; set; }

        //
        // Resumen:
        //     Pessoa Física / Pessoa Jurídica
        [Display(Name = "Tipo de pessoa")]
        [JsonProperty("person_type")]
        public string PersonType { get; set; }

        //
        // Resumen:
        //     Transferência automática
        [Display(Name = "Transferência automática")]
        [JsonProperty("automatic_transfer")]
        public bool AutomaticTransfer { get; set; }

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("cpf")]
        public string Cpf { get; set; }

        //
        // Resumen:
        //     Nome da empresa
        [Display(Name = "Nome da empresa")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        //
        // Resumen:
        //     Nome
        [Display(Name = "Nome")]
        [JsonProperty("name")]
        public string Name { get; set; }

        //
        // Resumen:
        //     Endereço
        [Display(Name = "Endereço")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("cep")]
        public string Cep { get; set; }

        //
        // Resumen:
        //     Cidade
        [Display(Name = "Cidade")]
        [JsonProperty("city")]
        public string City { get; set; }

        //
        // Resumen:
        //     Estado
        [Display(Name = "Estado")]
        [JsonProperty("state")]
        public string State { get; set; }

        //
        // Resumen:
        //     Telefone
        [Display(Name = "Telefone")]
        [JsonProperty("telphone")]
        public string Telphone { get; set; }

        //
        // Resumen:
        //     Responsável
        [Display(Name = "Responsável")]
        [JsonProperty("resp_name")]
        public string RespName { get; set; }

        //
        // Resumen:
        //     Cpf Responsável
        [Display(Name = "Cpf Responsável")]
        [JsonProperty("resp_cpf")]
        public string RespCpf { get; set; }

        //
        // Resumen:
        //     Banco
        [Display(Name = "Banco")]
        [JsonProperty("bank")]
        public string Bank { get; set; }

        //
        // Resumen:
        //     Agência
        [Display(Name = "Agência")]
        [JsonProperty("bank_ag")]
        public string BankAg { get; set; }

        //
        // Resumen:
        //     Agência (ignorar)
        [Display(Name = "Agência")]
        [JsonProperty("agency")]
        public string AgencyIgnored { get; set; }

        //
        // Resumen:
        //     Tipo de conta (CORRENTE / POUPANÇA)
        [Display(Name = "Tipo de conta")]
        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        //
        // Resumen:
        //     Conta
        [Display(Name = "Conta")]
        [JsonProperty("bank_cc")]
        public string BankCc { get; set; }

        public IuguAccountDataVerificationModel()
        {
            AutomaticValidation = true;
            AutomaticTransfer = true;
            PersonType = "Pessoa Física";
            AccountType = "Corrente";
            PhysicalProducts = false;
            PriceRange = "Mais que R$ 1,00";
            BusinessType = "Prestação de serviços";
        }
    }
}
