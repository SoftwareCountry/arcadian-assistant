namespace Arcadia.Assistant.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationDeviceModel
    {
        [Required]
        public string DevicePushToken { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(DeviceTypeEnum))]
        public DeviceTypeEnum DeviceType { get; set; } = DeviceTypeEnum.Android;
    }
}