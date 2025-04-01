using Moralar.UtilityFramework.Application.Core.ViewModels;

namespace Moralar.Domain.ViewModels
{
    public class FinishRaceViewModel : BaseViewModel
    {
        public double FinalDistance { get; set; }
        public string RoutePath { get; set; }
    }
}