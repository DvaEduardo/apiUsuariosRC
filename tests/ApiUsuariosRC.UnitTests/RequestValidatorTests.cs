using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Services.Validation;

namespace ApiUsuariosRC.UnitTests;

public class RequestValidatorTests
{
    [Fact]
    public void CrearUsuarioValidator_RejectsInvalidEmailAndMissingName()
    {
        var validator = new CrearUsuarioRequestDtoValidator();
        var result = validator.Validate(new CrearUsuarioRequestDto
        {
            GeneroId = 1,
            Nombres = "",
            ApellidoPaterno = "Villanueva",
            ApellidoMaterno = "Amaro",
            Correo = "correo-no-valido",
            Activo = true,
            FechaNacimiento = new DateTime(1996, 11, 16)
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CrearUsuarioRequestDto.Nombres));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CrearUsuarioRequestDto.Correo));
    }

    [Fact]
    public void BuscarUsuariosValidator_RejectsInvalidPagination()
    {
        var validator = new BuscarUsuariosRequestDtoValidator();
        var result = validator.Validate(new BuscarUsuariosRequestDto
        {
            Pagina = 0,
            TamanoPagina = 101
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(BuscarUsuariosRequestDto.Pagina));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(BuscarUsuariosRequestDto.TamanoPagina));
    }
}
