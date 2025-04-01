

using Moralar.Data.Entities;
using Moralar.Data.Enum;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using Moralar.UtilityFramework.Application.Core;
using static Moralar.Domain.Util;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyImportViewModel
    {

        [Display(Name = "Número do cadastro*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(1)]
        public string Numero_do_cadastro { get; set; }

        [Display(Name = "Nome do titular*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(2)]
        public string Nome_do_titular { get; set; }

        [Display(Name = "Cpf do titular*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [IsValidCpf(ErrorMessage = DefaultMessages.CpfInvalid)]
        [JsonConverter(typeof(OnlyNumber))]
        [Column(3)]
        public string Cpf_do_titular { get; set; }

        [Display(Name = "Data de nascimento*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [RegularExpression(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})( 00\:00\:00)?$", ErrorMessage = "Verifique o formato de dados enviado no campo \"{0}\", o mesmo deve ser formatado como texto")]
        [Column(4)]
        public string Data_de_Nascimento { get; set; }

        [Column(5)]
        [DropDownExcel(Options = typeof(TypeGenre), AllowBlank = true)]
        public string Genero { get; set; }

        [Display(Name = "E-mail*")]
        [EmailAddress(ErrorMessage = DefaultMessages.EmailInvalid)]
        [JsonConverter(typeof(ToLowerCase))]
        [Column(6)]
        public string E_mail { get; set; }

        [Display(Name = "Telefone")]
        [Column(7)]
        public string Telefone { get; set; }

        [Column(8)]
        [DropDownExcel(Options = typeof(TypeScholarity), AllowBlank = true)]
        public string Escolaridade { get; set; }

        [Display(Name = "Nome do Cônjuge")]
        [Column(9)]
        public string Nome_do_Conjuge { get; set; }

        [Display(Name = "Data de Nascimento")]
        [Column(10)]
        public string Data_de_Nascimento_do_Conjuge { get; set; }

        [Display(Name = "Gênero")]
        [DropDownExcel(Options = typeof(TypeGenre), AllowBlank = true)]
        [Column(11)]
        public string Genero_Conjuge { get; set; }

        [Display(Name = "Escolaridade")]
        [DropDownExcel(Options = typeof(TypeScholarity), AllowBlank = true)]
        [Column(12)]
        public string Escolaridade_Conjuge { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(13)]
        public string CEP { get; set; }

        [Display(Name = "Endereço*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(14)]
        public string StreetAddress { get; set; }

        [Display(Name = "Número*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(15)]
        public string Number { get; set; }

        [Display(Name = "Nome da cidade*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(16)]
        public string CityName { get; set; }

        [Display(Name = "Nome do estado*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(17)]
        public string StateName { get; set; }

        [Display(Name = "UF*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(18)]
        public string StateUf { get; set; }

        [Display(Name = "Complemento")]
        [Column(19)]
        public string Complement { get; set; }

        [Display(Name = "Bairro*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(20)]
        public string Neighborhood { get; set; }

        [Display(Name = "Renda familiar*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(21)]
        public string Renda_Familiar { get; set; }

        [Display(Name = "Valor imovel demolido*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(22)]
        public string Valor_imovel_demolido { get; set; }

        [Display(Name = "Valor para a compra do imovel*")]
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Column(23)]
        public string Valor_para_compra_de_imovel { get; set; }

        [Display(Name = "Valor incremento")]
        [Column(24)]
        public string Valor_Incremento { get; set; }

        [Display(Name = "Frente de obras")]
        [Column(25)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Frente_de_Obras { get; set; }

        [Display(Name = "Deficiencia que demande imovel acessivel")]
        [Column(26)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Deficiencia_que_demande_imovel_acessivel { get; set; }

        [Display(Name = "Idoso + de 80 anos")]
        [Column(27)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Idoso_acima_de_80_anos { get; set; }

        [Display(Name = "Idoso 60 - 79 anos")]
        [Column(28)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Idoso_60_ate_79_anos { get; set; }

        [Display(Name = "Mulher atendida por medida protetiva")]
        [Column(29)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Mulher_atendida_por_medida_protetiva { get; set; }

        [Display(Name = "Monoparental (pai e mae)")]
        [Column(30)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Monoparental { get; set; }

        [Display(Name = "Familia + de 5 pessoas")]
        [Column(31)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Familia_com_mais_5_pessoas { get; set; }

        [Display(Name = "Filhos menores de 18 anos")]
        [Column(32)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Filhos_menores_de_18_anos { get; set; }

        [Display(Name = "Chefe de familia sem renda")]
        [Column(33)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Chefe_de_familia_sem_renda { get; set; }

        [Display(Name = "Beneficio de prestacao continuada")]
        [Column(34)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Beneficio_de_prestacao_continuada { get; set; }

        [Display(Name = "Bolsa Familia")]
        [Column(35)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Bolsa_Familia { get; set; }

        [Display(Name = "Cobitacao involuntaria")]
        [Column(36)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Coabitacao_involuntaria { get; set; }

        [Display(Name = "Renda familiar de ate 2 salarios minimos")]
        [Column(37)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Renda_familiar_de_ate_dois_salarios_minimos { get; set; }

        [Display(Name = "Mulher chefe de familia")]
        [Column(38)]
        [DropDownExcel(Options = typeof(TypeAffirmation), AllowBlank = true)]
        public string Mulher_chefe_de_familia { get; set; }
    }
}

