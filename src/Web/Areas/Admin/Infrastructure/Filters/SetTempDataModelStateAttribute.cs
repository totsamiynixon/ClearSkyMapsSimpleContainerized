using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Web.Areas.Admin.Infrastructure.Filters
{
    public class SetTempDataModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var controller = (Controller)filterContext.Controller;
            controller.TempData["ModelState"] = JsonConvert.SerializeObject(controller.ViewData.ModelState);
        }
    }
}
