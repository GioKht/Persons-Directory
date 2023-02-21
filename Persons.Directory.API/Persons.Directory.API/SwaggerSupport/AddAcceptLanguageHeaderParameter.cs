using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Persons.Directory.API.SwaggerSupport;

public class AddAcceptLanguageHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var acceptLanguageHeader = new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("en-US")
            }
        };

        operation.Parameters.Add(acceptLanguageHeader);
    }
}
