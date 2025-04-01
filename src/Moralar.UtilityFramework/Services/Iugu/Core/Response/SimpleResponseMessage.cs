using Moralar.UtilityFramework.Services.Iugu.Core.Models;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Iugu.Core.Response
{
    public class SimpleResponseMessage : IuguBaseErrors
    {
        //
        // Resumen:
        //     Result of request
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
