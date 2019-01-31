namespace Arcadia.Assistant.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadAndroidWebController : Controller
    {
        [Route("download/android")]
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}