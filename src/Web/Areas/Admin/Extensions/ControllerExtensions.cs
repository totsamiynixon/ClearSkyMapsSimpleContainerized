using System.Net;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models.Default;

namespace Web.Areas.Admin.Extensions
{
    public static class ControllerExtensions
    {
        public static ViewResult StatusCodeView(this Controller controller, HttpStatusCode statusCode, string message)
        {
            var result = controller.View("StatusCodeResult", new StatusCodeViewModel(statusCode, message));
            result.StatusCode = (int)statusCode;
            return result;
        }
    }
}