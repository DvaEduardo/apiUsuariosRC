using ApiUsuariosRC.Api.Security;
using ApiUsuariosRC.Services.Interfaces;
using ApiUsuariosRC.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiUsuariosRC.Api.Filters;

public class ActiveEmployeeFilter : IAsyncActionFilter
{
    private readonly IEmployeeRequestValidator _employeeRequestValidator;

    public ActiveEmployeeFilter(IEmployeeRequestValidator employeeRequestValidator)
    {
        _employeeRequestValidator = employeeRequestValidator;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<SkipActiveEmployeeValidationAttribute>().Any())
        {
            await next();
            return;
        }

        var numeroEmpleado = context.HttpContext.Request.Headers[ApiSecurityConstants.NumeroEmpleadoHeaderName]
            .FirstOrDefault();

        var validation = await _employeeRequestValidator.ValidateAsync(
            numeroEmpleado,
            context.HttpContext.RequestAborted);

        if (validation.IsValid)
        {
            await next();
            return;
        }

        context.Result = validation.Status switch
        {
            EmployeeRequestValidationStatus.MissingNumeroEmpleado => new BadRequestObjectResult(new ProblemDetails
            {
                Title = "NumeroEmpleado requerido",
                Detail = $"Debe enviar el header {ApiSecurityConstants.NumeroEmpleadoHeaderName}.",
                Status = StatusCodes.Status400BadRequest
            }),
            _ => new ObjectResult(new ProblemDetails
            {
                Title = "Operacion no autorizada",
                Detail = "El NumeroEmpleado no existe o el usuario no esta activo.",
                Status = StatusCodes.Status403Forbidden
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            }
        };
    }
}
