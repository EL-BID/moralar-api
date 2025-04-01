using Moralar.Data.Enum;
using System.ComponentModel.DataAnnotations;

using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyMemberImportViewModel
    {
        [Display(Name = "Cpf Titular")]       
        [Column(1)]
        public string HolderCpf { get; set; }
        [Display(Name = "Nome do membro")]        
        [Column(2)]
        public string Name { get; set; }

        [Display(Name = "Data de nascimento (DD/MM/AAAA)")]
        [Column(3)]
        public string Birthday { get; set; }

        
        [Display(Name = " Gênero")]
        [Column(4)]
        [DropDownExcel(Options = typeof(TypeGenre), AllowBlank = true)]
        public string Genre { get; set; }
       
        [Display(Name = "Grau de parentesco")]             
        [Column(5)]
        [DropDownExcel(Options = typeof(TypeKingShip), AllowBlank = true)]
        public string KinShip { get; set; }
        
        [Display(Name = "Escolaridade")]
        [Column(6)]
        [DropDownExcel(Options = typeof(TypeScholarity), AllowBlank = true)]
        public string Scholarity { get; set; }
        
    }
}