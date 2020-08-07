using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ErrorController : Controller
    {
        public ActionResult Index(HttpStatusCode httpStatusCode, string message)
        {
            var exception = new Exception(message);
            exception.Data.Add("code", httpStatusCode);
            return View(exception);
        }
    }
}