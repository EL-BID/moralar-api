
namespace Moralar.UtilityFramework.Services.Core.Models
{
    public class Config
    {
        public SENDER SMTP { get; set; }

        public List<ONESIGNAL> ONESIGNAL { get; set; }

        public Config()
        {
            ONESIGNAL = new List<ONESIGNAL>();
            SMTP = new SENDER();
        }
    }
}
