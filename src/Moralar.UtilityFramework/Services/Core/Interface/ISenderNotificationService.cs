
using Moralar.UtilityFramework.Services.Core.Models;
using Newtonsoft.Json.Linq;

namespace Moralar.UtilityFramework.Services.Core.Interface
{
    public interface ISenderNotificationService
    {
        OneSignalResponse SendPush(string senderName, string message, IEnumerable<string> devicePushId, string groupName = null, string senderPhoto = null, JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, int messagelength = 150, int titleLength = 50);

        OneSignalResponse SendAllDevices(string senderName, string message, string groupName = null, string senderPhoto = null, string segments = "All", JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, int messagelength = 150, int titleLength = 50);

        OneSignalModel GetNotification(string notificationId, int indexKeys = 0);

        bool CancelPush(string notificationId, int indexKeys = 0);

        Task<OneSignalResponse> SendPushAsync(string senderName, string message, IEnumerable<string> devicePushId, string groupName = null, string senderPhoto = null, JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, bool configureAwait = false, int messagelength = 150, int titleLength = 50);

        Task<OneSignalResponse> SendAllDevicesAsync(string senderName, string message, string groupName = null, string senderPhoto = null, string segments = "All", JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, bool configureAwait = false, int messagelength = 150, int titleLength = 50);

        Task<OneSignalModel> GetNotificationAsync(string notificationId, int indexKeys = 0, bool configureAwait = false);

        Task<bool> CancelPushAsync(string notificationId, int indexKeys = 0, bool configureAwait = false);
    }
}
