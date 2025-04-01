using MongoDB.Bson;
using Moralar.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core;
using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyEditViewModel: BaseViewModel
    {

        /// <summary>
        /// Dados do Endereço
        /// </summary>
        [IsReadOnly]
        public FamilyAddressViewModel Address { get; set; }
        /// <summary>
        /// Dados do Titular da Família
        /// </summary>
        [IsReadOnly]
        public FamilyHolderMinViewModel Holder { get; set; }        

        /// <summary>
        /// Dados do Cônjuge
        /// </summary>
        [IsReadOnly]
        public FamilySpouseViewModel Spouse { get; set; }

        /// <summary>
        /// Dados do membro da Família
        /// </summary>
        [IsReadOnly]
        public List<FamilyMemberViewModel> Members { get; set; }

        /// <summary>
        /// Dados Financeiros
        /// </summary>
        [IsReadOnly]
        public FamilyFinancialViewModel Financial { get; set; }

        /// <summary>
        /// Dados de Priorização
        /// </summary>
        [IsReadOnly]
        public FamilyPriorizationViewModel Priorization { get; set; }

        /// <summary>
        /// Primeiro acesso
        /// </summary>      
        public bool IsFirstAcess { get; set; }
    }
}
