using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Data.Entities.Auxiliar
{
    public class FamilyFinancial
    {
        public decimal FamilyIncome { get; set; }
        public decimal PropertyValueForDemolished { get; set; }
        public decimal MaximumPurchase { get; set; }
        public decimal? IncrementValue { get; set; }
    }
}
