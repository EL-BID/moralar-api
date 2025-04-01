using System.Threading.Tasks;

using Hangfire.Server;

namespace Moralar.WebApi.HangFire.Interface
{
    public interface IHangFireService
    {
        Task MakeQuestionAvailable(PerformContext context = null);
        Task ScheduleAlert(PerformContext context = null);
    }
}