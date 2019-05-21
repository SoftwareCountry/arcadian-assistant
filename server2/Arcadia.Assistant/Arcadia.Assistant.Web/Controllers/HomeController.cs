using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Arcadia.Assistant.Web.Models;

namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Organization.Contracts;

    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var organizationClient = ServiceProxy.Create<IOrganization>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Organization"), new ServicePartitionKey(0));
            var message = await organizationClient.HelloWorldAsync();
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
