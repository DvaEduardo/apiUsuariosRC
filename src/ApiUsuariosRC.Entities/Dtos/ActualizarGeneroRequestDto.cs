using System.ComponentModel.DataAnnotations;

namespace ApiUsuariosRC.Entities.Dtos;

public class ActualizarGeneroRequestDto
{
    [Required]
    [StringLength(100)]
    public string Genero { get; set; } = string.Empty;
}
