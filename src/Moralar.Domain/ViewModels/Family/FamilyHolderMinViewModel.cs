using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyHolderMinViewModel
    {

        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Nome do titular")]
        [IsReadOnly]
        public string Name { get; set; }
        public TypeGenre? Genre { get; set; } = null;
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? Birthday { get; set; }
        public TypeScholarity? Scholarity { get; set; } = null;
        public string Number { get; set; }
        [IsReadOnly]
        public string Cpf { get; set; }
    }
}
