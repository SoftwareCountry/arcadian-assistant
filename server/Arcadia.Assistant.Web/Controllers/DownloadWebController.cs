namespace Arcadia.Assistant.Web.Controllers
{
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;

    [Route("/download")]
    public class DownloadWebController : Controller
    {
        private readonly IDownloadApplicationSettings downloadApplicationSettings;
        private readonly IHostingEnvironment hostingEnvironment;

        public DownloadWebController(
            IDownloadApplicationSettings downloadApplicationSettings,
            IHostingEnvironment hostingEnvironment)
        {
            this.downloadApplicationSettings = downloadApplicationSettings;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [Route("get/{applicationType}")]
        [HttpGet]
        public IActionResult GetFile(ApplicationType applicationType)
        {
            var model = new DownloadWebModel
            {
                AndroidDownloadPath = this.GetLatestFile(
                    this.downloadApplicationSettings.BuildsFolder,
                    this.downloadApplicationSettings.AndroidFilePattern),
                IosDownloadPath = this.GetLatestFile(
                    this.downloadApplicationSettings.BuildsFolder,
                    this.downloadApplicationSettings.IosFilePattern)
            };

            var filePath = applicationType == ApplicationType.Android
                ? model.AndroidDownloadPath
                : model.IosDownloadPath;
            var fileContentType = applicationType == ApplicationType.Android
                ? "application/vnd.android.package-archive"
                : "application/octet-stream";

            return this.PhysicalFile(filePath, fileContentType, Path.GetFileName(filePath));
        }

        private string GetLatestFile(string baseFolder, string filePattern)
        {
            var folder = Path.Combine(this.hostingEnvironment.ContentRootPath, baseFolder);
            if (!Directory.Exists(folder))
            {
                return null;
            }

            var files = Directory
                .GetFiles(folder, filePattern)
                .OrderByDescending(f => f);

            return files.FirstOrDefault();
        }

        public enum ApplicationType
        {
            Android,
            Ios
        }
    }
}