namespace Arcadia.Assistant.Web.Controllers.Builds
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using MobileBuild.Contracts;

    using Models;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class GetFileWebController : Controller
    {
        private readonly Dictionary<DeviceTypeEnum, string> fileContentTypeByDeviceType = new Dictionary<DeviceTypeEnum, string>
        {
            [DeviceTypeEnum.Android] = "application/vnd.android.package-archive",
            [DeviceTypeEnum.Ios] = "application/octet-stream"
        };

        private readonly ILogger logger;
        private readonly IMobileBuildActorFactory mobileBuildActor;
        private readonly IEmployees employees;

        public GetFileWebController(
            IEmployees employees,
            IMobileBuildActorFactory mobileBuildActor,
            ILogger<GetFileWebController> logger)
        {
            this.mobileBuildActor = mobileBuildActor;
            this.employees = employees;
            this.logger = logger;
        }

        [Route("get/android")]
        [HttpGet]
        public async Task<IActionResult> GetAndroid(CancellationToken cancellationToken)
        {
            var allEmployees = await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithNameFilter("serg"), cancellationToken);
            this.logger.LogDebug($"Employee count: {allEmployees.Length}");
            return await this.GetFile(DeviceTypeEnum.Android, cancellationToken);
        }

        [Route("get/ios")]
        [HttpGet]
        public Task<IActionResult> GetIos(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceTypeEnum.Ios, cancellationToken);
        }

        private async Task<IActionResult> GetFile(DeviceTypeEnum deviceType, CancellationToken cancellationToken)
        {
            var buildApplicationType = deviceType.MobileBuildType();
            this.logger.LogInformation("Request {MobileType} mobile build.", buildApplicationType);
            var downloadActor = this.mobileBuildActor.MobileBuild(buildApplicationType);

            var fileContentType = this.fileContentTypeByDeviceType[deviceType];
            var fileContent = await downloadActor.GetMobileBuildDataAsync(cancellationToken);
            this.logger.LogInformation("{MobileType} mobile build file received.", buildApplicationType);
            return this.File(fileContent, fileContentType);
        }
    }
}