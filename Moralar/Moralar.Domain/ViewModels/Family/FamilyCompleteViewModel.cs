﻿using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyCompleteViewModel : BaseViewModel
    {
        public string HolderNumber { get; set; }
        public string HolderName { get; set; }
        public string HolderCpf { get; set; }
        public long HolderBirthday { get; set; }
        public string HolderGenre { get; set; }
        public string HolderEmail { get; set; }
        public string HolderPhone { get; set; }
        public TypeScholarity HolderScholarity { get; set; }


        /// <summary>
        /// Dados do conjuge
        /// </summary>
        public string SpouseNumber { get; set; }
        public string SpouseName { get; set; }
        public string SpouseCpf { get; set; }
        public long SpouseBirthday { get; set; }
        public string SpouseGenre { get; set; }
        public string SpouseEmail { get; set; }
        public string SpousePhone { get; set; }
        public string SpouseRelationship { get; set; }
        public TypeScholarity SpouseScholarity { get; set; }


        /// <summary>
        /// Dados do membro da Família
        /// </summary>
        public string FamilyMemberNumber { get; set; }
        public string FamilyMemberName { get; set; }
        public string FamilyMemberCpf { get; set; }
        public long FamilyMemberBirthday { get; set; }
        public string FamilyMemberGenre { get; set; }
        public string FamilyMemberEmail { get; set; }
        public string FamilyMemberPhone { get; set; }
        public string FamilyMemberRelationship { get; set; }
        public TypeScholarity FamilyMemberScholarity { get; set; }
        public TypeKingShip FamilyKinShip { get; set; }
    }
}
