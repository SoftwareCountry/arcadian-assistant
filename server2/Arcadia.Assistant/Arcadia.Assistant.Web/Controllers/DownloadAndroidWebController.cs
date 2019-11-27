namespace Arcadia.Assistant.Web.Controllers
{
    using Arcadia.Assistant.AppCenterBuilds.Contracts;
    using Arcadia.Assistant.MobileBuild.Contracts;
    using Arcadia.Assistant.Web.Models;
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
            using CancellationTokenSource cts = new CancellationTokenSource();
            var version = await mobileBuildActor.MobileBuild(DeviceType.Android.MobileBuildType()).GetMobileBuildVersionAsync(cts.Token);
            return this.View(new HomeViewModel
            {
                HelpLink = this.helpSettings.HelpLink,
                Version = version
            });
        }
    }
}