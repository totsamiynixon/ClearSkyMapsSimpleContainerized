using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Areas.Admin.Controllers;

namespace Web.Areas.Admin.Filters
{
    public class AdminAreaHandleErrorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (filterContext.HttpContext.Response.StatusCode >= 400)
            {
                var message = (filterContext.Result as ObjectResult)?.Value?.ToString();
                filterContext.Result = new ErrorController().Index((HttpStatusCode)filterContext.HttpContext.Response.StatusCode, message);
                filterContext.Result.ExecuteResultAsync(filterContext);
            }
        }
    }
}
