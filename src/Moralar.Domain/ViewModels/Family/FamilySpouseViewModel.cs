using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilySpouseViewModel
    {
        [Display(Name = "Nome do cônjuge")]
        public string Name { get; set; }
        
        [Display(Name = "Data de nascimento")]
        public long? Birthday { get; set; }

        [Display(Name = "Gênero")]
        public TypeGenre? Genre { get; set; }

        [Display(Name = "Escolaridade")]
        public TypeScholarity? SpouseScholarity { get; set; }
    }
}
