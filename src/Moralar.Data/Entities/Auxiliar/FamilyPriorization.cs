using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Data.Entities.Auxiliar
{
    public class FamilyPriorization
    {
        public PriorityRate WorkFront { get; set; }
        public PriorityRate PermanentDisabled{ get; set; }
        public PriorityRate ElderlyOverEighty { get; set; }
        public PriorityRate WomanServedByProtectiveMeasure { get; set; }
        public PriorityRate FemaleBreadwinner { get; set; }
        public PriorityRate SingleParent { get; set; }
        public PriorityRate FamilyWithMoreThanFivePeople { get; set; }
        public PriorityRate ChildUnderEighteen { get; set; }
        public PriorityRate HeadOfHouseholdWithoutIncome { get; set; }
        public PriorityRate BenefitOfContinuedProvision { get; set; }
        public PriorityRate FamilyPurse { get; set; }
        public PriorityRate InvoluntaryCohabitation { get; set; }
        public PriorityRate FamilyIncomeOfUpTwoMinimumWages { get; set; }
        public PriorityRate YearsInSextyAndSeventyNine { get; set; }
    }

}
