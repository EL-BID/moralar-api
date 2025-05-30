﻿using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyCompleteViewModel : BaseViewModel
    {
        /// <summary>
        /// Dados do Endereço
        /// </summary>
        public FamilyAddressViewModel Address { get; set; }

        /// <summary>
        /// Dados do Titular
        /// </summary>
        public FamilyHolderViewModel Holder { get; set; }

        /// <summary>
        /// Dados do conjuge
        /// </summary>
        public FamilySpouseViewModel Spouse { get; set; }


        /// <summary>
        /// Dados do membro da Família
        /// </summary>
        public List<FamilyMemberViewModel> Members { get; set; }


        /// <summary>
        /// Dados Financeiros
        /// </summary>
        public FamilyFinancialViewModel Financial { get; set; }

        /// <summary>
        /// Dados de Priorização
        /// </summary>
        public FamilyPriorizationViewModel Priorization { get; set; }
        public string MotherName { get; set; }
        public string MotherCityBorned { get; set; }
        //public string Password { get; set; }
        public bool IsFirstAcess { get; set; }
        //public string ProviderId { get; set; }

    }
}
