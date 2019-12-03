namespace Arcadia.Assistant.Web.Controllers
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

            return deviceType == DeviceType.Android
                ? this.RedirectToAction("Index", "DownloadAndroidWeb")
                : this.RedirectToAction("Index", "DownloadIosWeb");
        }

        private DeviceType GetDeviceTypeByUserAgent(string userAgent)
        {
            var androidUserAgent = userAgent?.IndexOf("android", StringComparison.InvariantCultureIgnoreCase) ?? -1;
            if (androidUserAgent != -1)
            {
                return DeviceType.Android;
            }

            return DeviceType.Ios;
        }
    }
}