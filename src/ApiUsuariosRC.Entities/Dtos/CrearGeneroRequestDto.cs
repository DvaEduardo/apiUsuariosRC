using System.ComponentModel.DataAnnotations;

namespace ApiUsuariosRC.Entities.Dtos;

public class CrearGeneroRequestDto
{
    [Required]
    [StringLength(100)]
    public string Genero { get; set; } = string.Empty;
}
