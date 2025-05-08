using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // pega todos os atributos do endpoint (controller + action)
        var metadata = context.ApiDescription
                              .ActionDescriptor
                              .EndpointMetadata;

        var hasAuthorize = metadata.OfType<AuthorizeAttribute>().Any();
        var hasAllowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();

        // só continua se tiver [Authorize] e NÃO tiver [AllowAnonymous]
        if (!hasAuthorize || hasAllowAnonymous)
            return;

        // adiciona respostas padronizadas
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        // configura o security requirement pro Bearer
        var bearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [ bearerScheme ] = Array.Empty<string>()
            }
        };
    }
}
