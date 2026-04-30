namespace ApiUsuariosRC.Entities.Models;

public class Usuario
{
    public int UsuarioId { get; set; }

    public string NumeroEmpleado { get; set; } = string.Empty;

    public int GeneroId { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string ApellidoPaterno { get; set; } = string.Empty;

    public string ApellidoMaterno { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaNacimiento { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public Genero? Genero { get; set; }
}
