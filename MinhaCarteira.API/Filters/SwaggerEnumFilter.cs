using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinhaCarteira.API.Filters;

public class SwaggerEnumFilter : ISchemaFilter
{
  public void Apply(OpenApiSchema schema, SchemaFilterContext context)
  {
    if (context.Type.IsEnum)
    {
      schema.Enum.Clear();
      foreach (var enumValue in Enum.GetNames(context.Type))
      {
        schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(enumValue));
      }
    }
  }
}