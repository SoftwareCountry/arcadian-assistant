namespace Arcadia.Assistant.Web.Controllers
{
    using System.Text;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.Web.Properties;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadIosWebController : Controller
    {
        private readonly ISslSettings sslSettings;

        public DownloadIosWebController(ISslSettings sslSettings)
        {
            this.sslSettings = sslSettings;
        }

        [Route("download/ios")]
        [HttpGet]
        public IActionResult Index()
        {
            return this.View(new HomeViewModel
            {
                ManifestLink = this.GetAbsoluteUrl("GetIosManifest", "DownloadIosWeb")
            });
        }

        [Route("download/ios-manifest")]
        [HttpGet]
        public IActionResult GetIosManifest()
        {
            var getIosFileLink = this.GetAbsoluteUrl("GetIos", "GetFileWeb");

            var manifestContent = Resources.IosManifest
                .Replace("{downloadApplicationUrl}", getIosFileLink);

            return this.File(Encoding.UTF8.GetBytes(manifestContent), "text/html");
        }

        private string GetAbsoluteUrl(string actionName, string controllerName)
        {
            var absoluteUrl = this.sslSettings.SslOffloading
                ? this.Url.Action(actionName, controllerName, null, "https")
                : this.Url.Action(actionName, controllerName, null, this.Request.Scheme);

            return absoluteUrl;
        }
    }
}