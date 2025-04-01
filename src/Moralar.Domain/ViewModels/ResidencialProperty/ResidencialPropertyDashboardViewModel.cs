using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Domain.ViewModels.ResidencialProperty
{
    public class ResidencialPropertyDashboardViewModel
    {
        public int AmountFamilies { get; set; }
        public decimal PercentageAmoutReuniaoPgm { get; set; }
        public decimal PercentageAmoutChooseProperty { get; set; }

        public decimal PercentageAmoutChangeProperty { get; set; }
        public decimal PercentageAmoutPosMudanca { get; set; }

        public int AvailableForSale { get; set; }
        public int ResidencialPropertySaled { get; set; }

    }
}
