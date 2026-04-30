using System.ComponentModel.DataAnnotations;

namespace ApiUsuariosRC.Entities.Dtos;

public class ActualizarUsuarioRequestDto
{
    [Range(1, int.MaxValue)]
    public int UsuarioAccionId { get; set; }

    [Range(1, int.MaxValue)]
    public int GeneroId { get; set; }

    [Required]
    [StringLength(150)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ApellidoMaterno { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public bool Activo { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }
}
