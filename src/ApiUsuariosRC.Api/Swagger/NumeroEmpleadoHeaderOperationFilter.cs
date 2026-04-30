using ApiUsuariosRC.Api.Security;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiUsuariosRC.Api.Swagger;

public class NumeroEmpleadoHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<SkipActiveEmployeeValidationAttribute>().Any())
        {
            return;
        }

        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = ApiSecurityConstants.NumeroEmpleadoHeaderName,
            In = ParameterLocation.Header,
            Required = true,
            Description = "NumeroEmpleado activo que autoriza la operacion.",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}
