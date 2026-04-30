namespace ApiUsuariosRC.Services.Security;

public enum EmployeeRequestValidationStatus
{
    Valid,
    MissingNumeroEmpleado,
    InvalidOrInactiveNumeroEmpleado
}

public sealed class EmployeeRequestValidationResult
{
    public EmployeeRequestValidationResult(EmployeeRequestValidationStatus status)
    {
        Status = status;
    }

    public EmployeeRequestValidationStatus Status { get; }

    public bool IsValid => Status == EmployeeRequestValidationStatus.Valid;

    public static EmployeeRequestValidationResult Valid() => new(EmployeeRequestValidationStatus.Valid);

    public static EmployeeRequestValidationResult MissingNumeroEmpleado() => new(EmployeeRequestValidationStatus.MissingNumeroEmpleado);

    public static EmployeeRequestValidationResult InvalidOrInactiveNumeroEmpleado() => new(EmployeeRequestValidationStatus.InvalidOrInactiveNumeroEmpleado);
}
