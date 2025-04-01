using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels
{
    public class AddressMiniViewModel : BaseViewModel
    {
        public string FormatedAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}