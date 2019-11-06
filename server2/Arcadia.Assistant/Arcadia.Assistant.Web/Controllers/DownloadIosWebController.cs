namespace Arcadia.Assistant.Web.Controllers
{
    using System.Text;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadIosWebController : Controller
    {
        private readonly ISslSettings sslSettings;
        private readonly IHelpSettings helpSettings;
        private const string iosManifest = @"
    <value>&lt;?xml version=""1.0"" encoding=""UTF-8""?&gt;
&lt;!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd""&gt;
&lt;plist version = ""1.0"" & gt;
    &lt;dict&gt;
        &lt;key&gt;items&lt;/key&gt;
        &lt;array&gt;
            &lt;dict&gt;
                &lt;key&gt;assets&lt;/key&gt;
                &lt;array&gt;
                    &lt;dict&gt;
                        &lt;key&gt;kind&lt;/key&gt;
                        &lt;string&gt;software-package&lt;/string&gt;
                        &lt;key&gt;url&lt;/key&gt;
                        &lt;string&gt;{downloadApplicationUrl}&lt;/string&gt;
                    &lt;/dict&gt;
                &lt;/array&gt;
                &lt;key&gt;metadata&lt;/key&gt;
                &lt;dict&gt;
                    &lt;key&gt;bundle-identifier&lt;/key&gt;
                    &lt;string&gt;ru.spb.arcadia.arcadia-assistant&lt;/string&gt;
                    &lt;key&gt;bundle-version&lt;/key&gt;
                    &lt;string&gt;1.0&lt;/string&gt;
                    &lt;key&gt;kind&lt;/key&gt;
                    &lt;string&gt;software&lt;/string&gt;
                    &lt;key&gt;title&lt;/key&gt;
                    &lt;string&gt;ArcadiaAssistant&lt;/string&gt;
                &lt;/dict&gt;
            &lt;/dict&gt;
        &lt;/array&gt;
    &lt;/dict&gt;
&lt;/plist&gt;</value>";

        public DownloadIosWebController(ISslSettings sslSettings, IHelpSettings helpSettings)
        {
            this.sslSettings = sslSettings;
            this.helpSettings = helpSettings;
        }

        [Route("download/ios")]
        [HttpGet]
        public IActionResult Index()
        {
            return this.View(new HomeViewModel
            {
                ManifestLink = this.GetAbsoluteUrl("GetIosManifest", "DownloadIosWeb"),
                HelpLink = this.helpSettings.HelpLink
            });
        }

        [Route("download/ios-manifest")]
        [HttpGet]
        public IActionResult GetIosManifest()
        {
            var getIosFileLink = this.GetAbsoluteUrl("GetIos", "GetFileWeb");

            var manifestContent = iosManifest
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