namespace Arcadia.Assistant.Web.Controllers.Builds
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using MobileBuild.Contracts;

    using Models;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class GetFileWebController : Controller
    {
        private readonly IMobileBuildActorFactory mobileBuildActor;

        private readonly Dictionary<DeviceType, string> fileContentTypeByDeviceType = new Dictionary<DeviceType, string>
        {
            [DeviceType.Android] = "application/vnd.android.package-archive",
            [DeviceType.Ios] = "application/octet-stream"
        };

        public GetFileWebController(
            IMobileBuildActorFactory mobileBuildActor)
        {
            this.mobileBuildActor = mobileBuildActor;
        }

        [Route("get/android")]
        [HttpGet]
        public Task<IActionResult> GetAndroid()
        {
            return this.GetFile(DeviceType.Android);
        }

        [Route("get/ios")]
        [HttpGet]
        public Task<IActionResult> GetIos()
        {
            return this.GetFile(DeviceType.Ios);
        }

        private async Task<IActionResult> GetFile(DeviceType deviceType)
        {
            var buildApplicationType = deviceType.MobileBuildType();
            var downloadActor = this.mobileBuildActor.MobileBuild(buildApplicationType);

            var fileContentType = this.fileContentTypeByDeviceType[deviceType];
            var fileContent = await downloadActor.GetMobileBuildDataAsync(CancellationToken.None);
            return this.File(fileContent, fileContentType);
        }
    }
}