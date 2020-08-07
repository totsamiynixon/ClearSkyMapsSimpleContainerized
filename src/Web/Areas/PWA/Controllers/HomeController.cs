using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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