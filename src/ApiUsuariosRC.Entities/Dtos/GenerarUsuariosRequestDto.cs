using System.ComponentModel.DataAnnotations;

namespace ApiUsuariosRC.Entities.Dtos;

public class GenerarUsuariosRequestDto
{
    [Range(1, 500)]
    public int Cantidad { get; set; }

    [Range(1, int.MaxValue)]
    public int? GeneroId { get; set; }
}
