namespace Arcadia.Assistant.Web.Controllers
{
    using AppCenterBuilds.Contracts;
    using MobileBuild.Contracts;
    using Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadAndroidWebController : Controller
    {
        private readonly IMobileBuildActorFactory mobileBuildActor;
        private readonly IHelpSettings helpSettings;

        public DownloadAndroidWebController(IHelpSettings helpSettings, IMobileBuildActorFactory mobileBuildActor)
        {
            this.helpSettings = helpSettings;
            this.mobileBuildActor = mobileBuildActor;
        }

        [Route("download/android")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var version = await mobileBuildActor.MobileBuild(DeviceType.Android.MobileBuildType()).GetMobileBuildVersionAsync(CancellationToken.None);
            return this.View(new HomeViewModel
            {
                HelpLink = this.helpSettings.HelpLink,
                Version = version
            });
        }
    }
}