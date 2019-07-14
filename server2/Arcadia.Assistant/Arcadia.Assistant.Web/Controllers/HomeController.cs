﻿using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Arcadia.Assistant.Web.Models;

namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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