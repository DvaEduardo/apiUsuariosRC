namespace ApiUsuariosRC.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipActiveEmployeeValidationAttribute : Attribute
{
}
