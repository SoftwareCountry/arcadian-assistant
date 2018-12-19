namespace Arcadia.Assistant.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PushNotificationDeviceModel
    {
        [Required]
        public string DevicePushToken { get; set; }

        [Required]
        public DeviceTypeEnum DeviceType { get; set; }

        public enum DeviceTypeEnum
        {
            Android,
            Ios
        }
    }
}