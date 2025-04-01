using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Import
{
    public class ImportFamilyViewModel
    {
        [Column(1)]
        public string Numero_do_cadastro { get; set; }

        [Column(2)]
        public string Nome_do_titular { get; set; }

        public string Cpf_do_titular { get; set; }
        public long Data_de_nascimento { get; set; }
        public TypeGenre Genero { get; set; }
        public string E_mail { get; set; }
        public string Telefone { get; set; }
        public TypeScholarity Escolaridade { get; set; }
        public string Nome_do_Conjuge { get; set; }
        public long Data_de_Nascimento { get; set; }
        public TypeGenre Genero_Conjuge { get; set; }
        public TypeScholarity Escolaridade_Conjuge { get; set; }
        public string Nome_do_Membro_1 { get; set; }
        public long Data_de_Nascimento_Membro_1 { get; set; }
        public TypeGenre Genero_Membro_1 { get; set; }
        public TypeKingShip Grau_De_Parentesco_Membro_1 { get; set; }
        public TypeScholarity Escolaridade_Membro_2 { get; set; }
        public string Nome_do_Membro_2 { get; set; }
        public long Data_de_Nascimento_Membro_2 { get; set; }
        public TypeGenre Genero_Membro_2 { get; set; }
        public TypeKingShip Grau_de_parentesco_Membro_2 { get; set; }
        public string Nome_do_Membro_3 { get; set; }
        public long Data_de_Nascimento_Membro_3 { get; set; }
        public TypeGenre Genero_Membro_3 { get; set; }
        public TypeKingShip Grau_de_parentesco_Membro_3 { get; set; }
        public TypeScholarity Escolaridade_Membro_3 { get; set; }
        public string Nome_do_Membro_4 { get; set; }
        public long Data_de_Nascimento_Membro_4 { get; set; }
        public TypeGenre Genero_Membro_4 { get; set; }
        public TypeKingShip Grau_de_parentesco_Membro_4 { get; set; }
        public TypeScholarity Escolaridade_Membro_4 { get; set; }
        public string Nome_do_Membro_5 { get; set; }
        public long Data_de_Nascimento_Membro_5 { get; set; }
        public TypeGenre Gênero_Membro_5 { get; set; }
        public TypeKingShip Grau_de_parentesco_Membro_5 { get; set; }
        public TypeScholarity Escolaridade_Membro_5 { get; set; }
        public decimal Renda_Familiar { get; set; }
        public decimal Valor_imovel_demolido { get; set; }
        public decimal Valor_para_compra_de_imovel { get; set; }
        public decimal Valor_incremento { get; set; }
        public bool Frente_de_Obras { get; set; }
        public bool Deficiencia_que_demande_imovel_acessivel { get; set; }
        public bool Idoso_acima_de_80_anos { get; set; }
        public bool Idoso_60_ate_79_anos { get; set; }
        public bool Mulher_atendida_por_medida_protetiva { get; set; }
        public bool Monoparental { get; set; }
        public bool Familia_com_mais_5_pessoas { get; set; }
        public bool Filhos_menores_de_18_anos { get; set; }
        public bool Chefe_de_familia_sem_renda { get; set; }
        public bool Beneficio_de_prestacao_continuada { get; set; }
        public bool Bolsa_Familia { get; set; }
        public bool Coabitacao_involuntaria { get; set; }
        public bool Renda_familiar_de_ate_dois_salarios_minimos { get; set; }
    }
}
