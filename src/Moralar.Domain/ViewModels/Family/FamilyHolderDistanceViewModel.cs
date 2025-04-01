using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Moralar.Domain.ViewModels.Family
{
    public class FamilyHolderDistanceViewModel
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string Cpf { get; set; }

        public string AddressPropertyOrigin { get; set; }
        public string AddressPropertyDestination { get; set; }
        public double AddressPropertyDistanceMeters { get; set; }
        public double AddressPropertyDistanceKilometers { get; set; }       
        public double OriginLatitude { get; set; }       
        public double OriginLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }
    }
}
