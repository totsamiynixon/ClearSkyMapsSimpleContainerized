using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Extensions;

namespace Web.Areas.Admin.Controllers.Default
{
    [Area(AdminArea.Name)]
    public class ErrorController : Controller
    {
        public IActionResult ServerError(int code = 500)
        {
            return this.StatusCodeView(HttpStatusCode.InternalServerError, "Internal Server Error");
        }
    }
}