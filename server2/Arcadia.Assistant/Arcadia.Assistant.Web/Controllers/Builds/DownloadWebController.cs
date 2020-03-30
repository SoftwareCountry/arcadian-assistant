namespace Arcadia.Assistant.Web.Controllers.Builds
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    using Models;

    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadWebController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var userAgent = this.Request.Headers[HeaderNames.UserAgent];
            var deviceType = this.GetDeviceTypeByUserAgent(userAgent);

            return deviceType == DeviceTypeEnum.Android
                ? this.RedirectToAction("Index", "DownloadAndroidWeb")
                : this.RedirectToAction("Index", "DownloadIosWeb");
        }

        private DeviceTypeEnum GetDeviceTypeByUserAgent(string userAgent)
        {
            var androidUserAgent = userAgent?.IndexOf("android", StringComparison.InvariantCultureIgnoreCase) ?? -1;
            if (androidUserAgent != -1)
            {
                return DeviceTypeEnum.Android;
            }

            return DeviceTypeEnum.Ios;
        }
    }
}