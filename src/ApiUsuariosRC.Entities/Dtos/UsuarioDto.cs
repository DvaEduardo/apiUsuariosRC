namespace ApiUsuariosRC.Entities.Dtos;

public class UsuarioDto
{
    public int UsuarioId { get; set; }

    public string NumeroEmpleado { get; set; } = string.Empty;

    public int GeneroId { get; set; }

    public string Genero { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string ApellidoPaterno { get; set; } = string.Empty;

    public string ApellidoMaterno { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}
