using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.Areas
{
    public interface IArea
    {
        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder app);
    }

    public interface ISwaggerSupportArea : IArea
    {
        void ConfigureSwagger(SwaggerGenOptions options);
        
        Func<string, ApiDescription, bool> SwaggerInclusionPredicate { get; }
    }
}