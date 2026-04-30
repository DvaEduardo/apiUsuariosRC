namespace ApiUsuariosRC.Entities.Dtos;

public class UsuarioConHistorialDto : UsuarioDto
{
    public IReadOnlyCollection<UsuarioCambioLogDto> HistorialCambios { get; set; } = [];
}
