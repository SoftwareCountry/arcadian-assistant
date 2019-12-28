namespace Arcadia.Assistant.Web.Models
{
    using System.Collections.Generic;

    using MobileBuild.Contracts;

    public enum DeviceType
    {
        Android,
        Ios
    }

    internal static class DeviceTypeExtension
    {
        private static readonly Dictionary<DeviceType, string> BuildTypeByDeviceType =
            new Dictionary<DeviceType, string>
            {
                [DeviceType.Android] = WellKnownBuildTypes.Android,
                [DeviceType.Ios] = WellKnownBuildTypes.Ios
            };

        public static string MobileBuildType(this DeviceType type)
        {
            return BuildTypeByDeviceType[type];
        }
    }
}