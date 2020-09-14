using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Infrastructure.Middlewares
{
    //TODO: Important! Check Error handling https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1
    //TODO: Important! Problem Details object
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error handler will not be executed.");
                }
                
                context.Response.Clear();
                context.Response.Headers.Clear();
                context.Response.StatusCode = 500;

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    context,
                    statusCode: 500,
                    title: "Internal Server Error");

                await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                }));

            }
        }
    }
}
