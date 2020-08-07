using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Web.Areas.Admin.Infrastructure.Filters
{
    public class RestoreModelStateFromTempDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controller = (Controller)filterContext.Controller;
            if (controller.TempData.ContainsKey("ModelState"))
            {
                controller.ViewData.ModelState.Merge(
                    JsonConvert.DeserializeObject<ModelStateDictionary>(controller.TempData["ModelState"]?.ToString()));
            }
        }
    }
}
