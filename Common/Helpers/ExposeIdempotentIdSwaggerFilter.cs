using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Helpers;

public class ExposeIdempotentIdSwaggerFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter{
            Name = "idempotenceId",
            In = ParameterLocation.Query,
            Required = true,
            Schema = new OpenApiSchema {
                Type = "string",
                Format = "uuid",
                Example = new OpenApiString(Guid.NewGuid().ToString()),
            }
            
        });
    }
}