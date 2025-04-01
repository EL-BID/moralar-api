using System;
using System.Collections.Generic;
using System.Text;

namespace Moralar.Data.Entities.Auxiliar
{
    public class ResidencialPropertyAdress
    {
        public string StreetAddress { get; set; }
        public string Number { get; set; }
        public string CityName { get; set; }
        public string CityId { get; set; }
        public string StateName { get; set; }
        public string StateUf { get; set; }
        public string StateId { get; set; }
        public string Neighborhood { get; set; }
        public string Complement { get; set; }
        public string Location { get; set; }
        public string CEP { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
