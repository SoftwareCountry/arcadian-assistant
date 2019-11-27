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
        private static Dictionary<DeviceType, ApplicationType> buildTypeByDeviceType =
            new Dictionary<DeviceType, ApplicationType>
            {
                [DeviceType.Android] = ApplicationType.Android,
                [DeviceType.Ios] = ApplicationType.Ios
            };

        public static string MobileBuildType(this DeviceType type)
        {
            return buildTypeByDeviceType[type].ToString();
        }
    }
}