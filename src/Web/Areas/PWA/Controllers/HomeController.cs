using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Helpers.Interfaces;

namespace Web.Areas.PWA.Controllers
{
    [Area("PWA")]
    [Route("")]
    public class HomeController : Controller
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISettingsProvider _settingsProvider;
        public HomeController(IHostingEnvironment hostingEnvironment, ISettingsProvider settingsProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _settingsProvider = settingsProvider;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}