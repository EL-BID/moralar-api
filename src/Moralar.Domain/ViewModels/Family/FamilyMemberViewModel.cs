using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyMemberViewModel 
    {
        [Display(Name = "Nome do membro")]
        [IsReadOnly]
        public string Name { get; set; }
       

        [Display(Name = "Data de nascimento")]
        [IsReadOnly]
        public long Birthday { get; set; }

        
        [Display(Name = " Gênero")]
        public TypeGenre Genre { get; set; }

        
        [Display(Name = "Grau de parentesco")]
        public TypeKingShip KinShip { get; set; }


        [Display(Name = "Escolaridade")]
        public TypeScholarity Scholarity { get; set; }
    }
}
