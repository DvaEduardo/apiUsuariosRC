using ApiUsuariosRC.Services.Security;

namespace ApiUsuariosRC.Services.Interfaces;

public interface IEmployeeRequestValidator
{
    Task<EmployeeRequestValidationResult> ValidateAsync(string? numeroEmpleado, CancellationToken cancellationToken = default);
}
