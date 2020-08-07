using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Web.Infrastructure.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionHandlerMiddleware>();
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.Headers.Clear();
                context.Response.StatusCode = 500;
                if (_hostingEnvironment.IsDevelopment())
                {
                    JsonResult result = new JsonResult(ex, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    RouteData routeData = context.GetRouteData();
                    ActionDescriptor actionDescriptor = new ActionDescriptor();
                    ActionContext actionContext = new ActionContext(context, routeData ?? new RouteData(), actionDescriptor);
                    await result.ExecuteResultAsync(actionContext);
                    return;
                }
                _logger.LogError(ex, "An unhandled exception has occurred: " + ex.Message);
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error handler will not be executed.");
                }
            }
        }
    }
}
