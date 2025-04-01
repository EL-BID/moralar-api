
namespace Moralar.UtilityFramework.Services.Core.Models
{
    public class SENDER
    {
        public string TEMPLATE { get; set; }

        public string HOST { get; set; }

        public int PORT { get; set; }

        public bool SSL { get; set; }

        public string EMAIL { get; set; }

        public string NAME { get; set; }

        public string PASSWORD { get; set; }

        public string LOGIN { get; set; }

        public bool USEAUTH { get; set; } = true;

    }
}
