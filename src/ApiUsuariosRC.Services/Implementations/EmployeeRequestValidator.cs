using ApiUsuariosRC.DBOperation.Repositories;
using ApiUsuariosRC.Services.Interfaces;
using ApiUsuariosRC.Services.Security;

namespace ApiUsuariosRC.Services.Implementations;

public class EmployeeRequestValidator : IEmployeeRequestValidator
{
    private readonly IUsuarioRepository _usuarioRepository;

    public EmployeeRequestValidator(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<EmployeeRequestValidationResult> ValidateAsync(
        string? numeroEmpleado,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(numeroEmpleado))
        {
            return EmployeeRequestValidationResult.MissingNumeroEmpleado();
        }

        var usuario = await _usuarioRepository.GetActiveByNumeroEmpleadoAsync(numeroEmpleado.Trim(), cancellationToken);
        return usuario is null
            ? EmployeeRequestValidationResult.InvalidOrInactiveNumeroEmpleado()
            : EmployeeRequestValidationResult.Valid();
    }
}
