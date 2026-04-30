using System.ComponentModel.DataAnnotations;

namespace ApiUsuariosRC.Entities.Dtos;

public class BajaUsuarioRequestDto
{
    [Range(1, int.MaxValue)]
    public int UsuarioAccionId { get; set; }
}
