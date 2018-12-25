namespace Arcadia.Assistant.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PushNotificationDeviceModel
    {
        [Required]
        public string DevicePushToken { get; set; }

        [Required]
        [EnumDataType(typeof(DeviceType))]
        public DeviceType DeviceType { get; set; }
    }
}