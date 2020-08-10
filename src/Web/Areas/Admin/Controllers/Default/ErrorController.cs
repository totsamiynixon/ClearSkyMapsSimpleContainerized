using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers.Default
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