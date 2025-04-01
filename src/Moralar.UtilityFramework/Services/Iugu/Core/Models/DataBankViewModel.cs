
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Models
{
    public class DataBankViewModel
    {
        //
        // Resumen:
        //     NOME FANTASIA
        [Display(Name = "Nome fantasia")]
        public string FantasyName { get; set; }

        //
        // Resumen:
        //     RAZÃO SOCIAL
        [Display(Name = "Razão social")]
        public string SocialName { get; set; }

        //
        // Resumen:
        //     NUMERO DA CONTA
        [Display(Name = "Número da conta")]
        public string BankAccount { get; set; }

        //
        // Resumen:
        //     NUMERO DA AGENCIA
        [Display(Name = "Agência")]
        public string BankAgency { get; set; }

        //
        // Resumen:
        //     NOME DO BANCO OU CODIGO
        [Display(Name = "Banco")]
        public string Bank { get; set; }

        //
        // Resumen:
        //     DEFAULT corrente | 'Corrente' 'Poupança'
        [Display(Name = "Tipo de conta")]
        public string AccountType { get; set; }

        //
        // Resumen:
        //     ENDEREÇO
        [Display(Name = "Endereço")]
        public string Address { get; set; }

        //
        // Resumen:
        //     CASO PESSOA JURIDICA ENVIAR CPF DO TITULAR DA CONTA
        [Display(Name = "Titular da conta")]
        public string AccountableCpf { get; set; }

        //
        // Resumen:
        //     CASO PESSOA FISICA
        public string Cpf { get; set; }

        [Display(Name = "Telefone responsável")]
        public string AccountablePhone { get; set; }

        //
        // Resumen:
        //     DEFAULT | Pessoa Fisíca | TYPES = 'Pessoa Física' ou 'Pessoa Jurídica'
        public string PersonType { get; set; }

        //
        // Resumen:
        //     Descrição do negócio
        [Display(Name = "Descrição do negócio")]
        public string BusinessType { get; set; }

        //
        // Resumen:
        //     CASO PESSOA JURIDICA
        [Display(Name = "Cnpj")]
        public string Cnpj { get; set; }

        //
        // Resumen:
        //     DEFAULT = "Mais que R$ 1,00" | EX: Valor máximo da venda ('Até R$ 100,00', 'Entre
        //     R$ 100,00 e R$ 500,00', 'Mais que R$ 500,00')
        [Display(Name = "Valor entre")]
        public string PriceRange { get; set; }

        //
        // Resumen:
        //     VERIFICAÇÃO AUTOMATICA | DEFAULT = TRUE
        [Display(Name = "Verificação automatica")]
        public bool AutomaticValidation { get; set; }

        //
        // Resumen:
        //     SOLICITAR SAQUE AUTOMATICO | DEFAULT = TRUE
        [Display(Name = "Solicitar saque automatico")]
        public bool AutomaticTransfer { get; set; }

        //
        // Resumen:
        //     VENDE PRODUTOS FISICOS
        [Display(Name = "Vende produtos físicos")]
        public bool PhysicalProducts { get; set; }

        //
        // Resumen:
        //     CEP
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        //
        // Resumen:
        //     CIDADE
        [Display(Name = "Cidade")]
        public string City { get; set; }

        //
        // Resumen:
        //     ESTADO
        [Display(Name = "Estado")]
        public string State { get; set; }

        //
        // Resumen:
        //     DATA DA ULTIMA VERIFICAÇÃO (UNIX)
        [Display(Name = "Data da Última verificação")]
        public long? LastRequestVerification { get; set; }

        //
        // Resumen:
        //     RESPONSAVEL PELA CONTA
        [Display(Name = "Responsável")]
        public string AccountableName { get; set; }

        public DataBankViewModel()
        {
            PersonType = "Pessoa Física";
            AccountType = "Corrente";
            AutomaticValidation = true;
            AutomaticTransfer = true;
            PriceRange = "Mais que R$ 1,00";
            BusinessType = "Prestador de serviço";
            Address = "Av Paulista, 2202";
            Cep = "01310-100";
            City = "São Paulo";
            State = "SP";
        }
    }

}
