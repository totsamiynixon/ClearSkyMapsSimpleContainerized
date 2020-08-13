using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.PWA.Controllers
{
    [Area(PWAArea.Name)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}