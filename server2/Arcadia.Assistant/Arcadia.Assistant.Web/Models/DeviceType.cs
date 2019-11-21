using Arcadia.Assistant.AppCenterBuilds.Contracts;
using System.Collections.Generic;

namespace Arcadia.Assistant.Web.Models
{
    public enum DeviceType
    {
        Android,
        Ios
    }

    internal static class DeviceTypeExtension
    {
        private static Dictionary<DeviceType, ApplicationTypeEnum> buildTypeByDeviceType =
            new Dictionary<DeviceType, ApplicationTypeEnum>
            {
                [DeviceType.Android] = ApplicationTypeEnum.Android,
                [DeviceType.Ios] = ApplicationTypeEnum.Ios
            };

        public static string MobileBuildType(this DeviceType type) 
        { 
            return buildTypeByDeviceType[type].ToString(); 
        }
    }
}