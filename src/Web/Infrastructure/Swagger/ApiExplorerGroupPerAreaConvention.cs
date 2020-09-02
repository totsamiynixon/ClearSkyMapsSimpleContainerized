using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Web.Infrastructure.Swagger
{
    public class ApiExplorerGroupPerAreaConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            
            var controllerAreaAttribute = controller.ControllerType.GetCustomAttributes(true).OfType<AreaAttribute>().FirstOrDefault();
            var controllerArea = controllerAreaAttribute?.RouteValue;

            if (!string.IsNullOrEmpty(controllerArea))
            {
                controller.ApiExplorer.GroupName = controllerArea;
            }
        }
    }
}