using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Moralar.UtilityFramework.Application.Core;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyPriorizationViewModel
    {
        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Frente de obras")]        
        public PriorityRateViewModel WorkFront { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Deficiência permanente que demande imóvel acessível")]
        public PriorityRateViewModel PermanentDisabled { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Idoso mais de 80 anos")]        
        public PriorityRateViewModel ElderlyOverEighty { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Idoso 60 - 79 anos")]        
        public PriorityRateViewModel YearsInSextyAndSeventyNine { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Mulher atendida por medida protetiva")]        
        public PriorityRateViewModel WomanServedByProtectiveMeasure { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Mulher chefe de família")]       
        public PriorityRateViewModel FemaleBreadWinner { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Monoparental (pai e mãe)")]       
        public PriorityRateViewModel SingleParent { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Família com mais de 5 pessoas")]       
        public PriorityRateViewModel FamilyWithMoreThanFivePeople { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Filho(s) menor(es) de 18 anos na composição familiar")]       
        public PriorityRateViewModel ChildUnderEighteen { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Chefe de família sem renda *")]       
        public PriorityRateViewModel HeadOfHouseholdWithoutIncome { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Benefício de prestação continuada - BPC")]       
        public PriorityRateViewModel BenefitOfContinuedProvision { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Bolsa família")]       
        public PriorityRateViewModel FamilyPurse { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Coabitação involuntária")]       
        public PriorityRateViewModel InvoluntaryCohabitation { get; set; }


        [Required(ErrorMessage = DefaultMessages.FieldRequired)]
        [Display(Name = "Renda familiar de até 2 salários mínimos")]       
        public PriorityRateViewModel FamilyIncomeOfUpTwoMinimumWages { get; set; }

    }
}
