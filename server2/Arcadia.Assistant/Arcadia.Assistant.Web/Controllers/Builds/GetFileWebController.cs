namespace Arcadia.Assistant.Web.Controllers.Builds
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using MobileBuild.Contracts;

    using Models;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class GetFileWebController : Controller
    {
        private readonly Dictionary<DeviceType, string> fileContentTypeByDeviceType = new Dictionary<DeviceType, string>
        {
            [DeviceType.Android] = "application/vnd.android.package-archive",
            [DeviceType.Ios] = "application/octet-stream"
        };

        private readonly ILogger logger;
        private readonly IMobileBuildActorFactory mobileBuildActor;

        public GetFileWebController(
            IMobileBuildActorFactory mobileBuildActor,
            ILogger<GetFileWebController> logger)
        {
            this.mobileBuildActor = mobileBuildActor;
            this.logger = logger;
        }

        [Route("get/android")]
        [HttpGet]
        public Task<IActionResult> GetAndroid(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Android, cancellationToken);
        }

        [Route("get/ios")]
        [HttpGet]
        public Task<IActionResult> GetIos(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Ios, cancellationToken);
        }

        private async Task<IActionResult> GetFile(DeviceType deviceType, CancellationToken cancellationToken)
        {
            var buildApplicationType = deviceType.MobileBuildType();
            this.logger.LogInformation($"Request {buildApplicationType} mobile build.");
            var downloadActor = this.mobileBuildActor.MobileBuild(buildApplicationType);

            var fileContentType = this.fileContentTypeByDeviceType[deviceType];
            var fileContent = await downloadActor.GetMobileBuildDataAsync(cancellationToken);
            this.logger.LogInformation($"{buildApplicationType} mobile build file received.");
            return this.File(fileContent, fileContentType);
        }
    }
}