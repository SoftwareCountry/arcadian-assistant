namespace Arcadia.Assistant.Web.Models
{
    using System.Collections.Generic;

    using MobileBuild.Contracts;

    public enum DeviceTypeEnum
    {
        Android,
        Ios
    }

    internal static class DeviceTypeExtension
    {
        private static readonly Dictionary<DeviceTypeEnum, string> BuildTypeByDeviceType =
            new Dictionary<DeviceTypeEnum, string>
            {
                [DeviceTypeEnum.Android] = WellKnownBuildTypes.Android,
                [DeviceTypeEnum.Ios] = WellKnownBuildTypes.Ios
            };

        public static string MobileBuildType(this DeviceTypeEnum type)
        {
            return BuildTypeByDeviceType[type];
        }
    }
}