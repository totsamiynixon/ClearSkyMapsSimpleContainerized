using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Web.Areas.Admin.Infrastructure.Middlewares
{
    public class StatusCodeHanlderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StatusCodeHanlderMiddleware> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public StatusCodeHanlderMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment,
            ILogger<StatusCodeHanlderMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stream originalBody = context.Response.Body;
            string responseBody;
            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);

                    memStream.Position = 0;
                    responseBody = await new StreamReader(memStream).ReadToEndAsync();


                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }

            if (context.Response.StatusCode >= 400)
            {
                context.Request.Path =
                    $"/admin/statuscode?code={context.Response.StatusCode}&message={responseBody}";
                await _next(context);
            }
        }
    }
}