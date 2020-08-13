using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Web.Infrastructure.Middlewares
{
    public class ExceptionLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggerMiddleware> _logger;

        public ExceptionLoggerMiddleware(RequestDelegate next, ILogger<ExceptionLoggerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred: " + ex.Message);
                
                throw;
            }
        }
    }
}
