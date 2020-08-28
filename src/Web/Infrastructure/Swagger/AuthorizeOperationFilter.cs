using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.Infrastructure.Swagger
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            context.ApiDescription.TryGetMethodInfo(out var methodInfo);

            if (methodInfo == null)
                return;

            var isAuthorized = false;
            AuthorizeAttribute authorizeAttribute = null;

            if (methodInfo.MemberType == MemberTypes.Method)
            {
                 authorizeAttribute = methodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                    .FirstOrDefault();
                isAuthorized = authorizeAttribute != null;
 
                // NOTE: Controller has Authorize attribute, so check the endpoint itself.
                //       Take into account the allow anonymous attribute
                if (isAuthorized)
                    isAuthorized =
                        !methodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();
                else
                {
                    authorizeAttribute = methodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                        .FirstOrDefault(); 
                        isAuthorized = authorizeAttribute != null;
                }
            }

            if (!isAuthorized)
                return;

            if (operation.Responses.All(r => r.Key != StatusCodes.Status401Unauthorized.ToString()))
                operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(),
                    new OpenApiResponse {Description = "Unauthorized"});
            
            var securityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = authorizeAttribute.AuthenticationSchemes }
            };
            
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ securityScheme ] = new List<string> { authorizeAttribute.Policy }
                }
            };
        }
    }
}