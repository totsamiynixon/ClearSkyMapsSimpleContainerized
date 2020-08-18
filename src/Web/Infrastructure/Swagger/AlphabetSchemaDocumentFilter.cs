using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.Infrastructure.Swagger
{
    public class AlphabetSchemaDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Components.Schemas = swaggerDoc.Components.Schemas.OrderBy(z => z.Key)
                .ToDictionary(z => z.Key, v => v.Value);
        }
    }
}