using ApiUsuariosRC.Entities.Dtos;
using FluentValidation;

namespace ApiUsuariosRC.Services.Validation;

public class CrearUsuarioRequestDtoValidator : AbstractValidator<CrearUsuarioRequestDto>
{
    public CrearUsuarioRequestDtoValidator()
    {
        RuleFor(x => x.GeneroId).GreaterThan(0);
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ApellidoPaterno).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ApellidoMaterno).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Correo).NotEmpty().MaximumLength(150).EmailAddress();
        RuleFor(x => x.FechaNacimiento).NotEqual(default(DateTime));
    }
}

public class ActualizarUsuarioRequestDtoValidator : AbstractValidator<ActualizarUsuarioRequestDto>
{
    public ActualizarUsuarioRequestDtoValidator()
    {
        RuleFor(x => x.UsuarioAccionId).GreaterThan(0);
        RuleFor(x => x.GeneroId).GreaterThan(0);
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ApellidoPaterno).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ApellidoMaterno).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Correo).NotEmpty().MaximumLength(150).EmailAddress();
        RuleFor(x => x.FechaNacimiento).NotEqual(default(DateTime));
    }
}

public class BajaUsuarioRequestDtoValidator : AbstractValidator<BajaUsuarioRequestDto>
{
    public BajaUsuarioRequestDtoValidator()
    {
        RuleFor(x => x.UsuarioAccionId).GreaterThan(0);
    }
}

public class BuscarUsuariosRequestDtoValidator : AbstractValidator<BuscarUsuariosRequestDto>
{
    public BuscarUsuariosRequestDtoValidator()
    {
        RuleFor(x => x.UsuarioId).GreaterThan(0).When(x => x.UsuarioId.HasValue);
        RuleFor(x => x.NombreOCorreo).MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.NombreOCorreo));
        RuleFor(x => x.Pagina).GreaterThan(0);
        RuleFor(x => x.TamanoPagina).InclusiveBetween(1, 100);
    }
}

public class GenerarUsuariosRequestDtoValidator : AbstractValidator<GenerarUsuariosRequestDto>
{
    public GenerarUsuariosRequestDtoValidator()
    {
        RuleFor(x => x.Cantidad).InclusiveBetween(1, 500);
        RuleFor(x => x.GeneroId).GreaterThan(0).When(x => x.GeneroId.HasValue);
    }
}

public class CrearGeneroRequestDtoValidator : AbstractValidator<CrearGeneroRequestDto>
{
    public CrearGeneroRequestDtoValidator()
    {
        RuleFor(x => x.Genero).NotEmpty().MaximumLength(100);
    }
}

public class ActualizarGeneroRequestDtoValidator : AbstractValidator<ActualizarGeneroRequestDto>
{
    public ActualizarGeneroRequestDtoValidator()
    {
        RuleFor(x => x.Genero).NotEmpty().MaximumLength(100);
    }
}

public class PaginacionRequestDtoValidator : AbstractValidator<PaginacionRequestDto>
{
    public PaginacionRequestDtoValidator()
    {
        RuleFor(x => x.Pagina).GreaterThan(0);
        RuleFor(x => x.TamanoPagina).InclusiveBetween(1, 100);
    }
}
