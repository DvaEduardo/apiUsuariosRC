using System.ComponentModel.DataAnnotations;

namespace ApiUsuariosRC.Entities.Dtos;

public class BuscarUsuariosRequestDto
{
    public int? UsuarioId { get; set; }

    public string? NombreOCorreo { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaCreacion { get; set; }

    [Range(1, int.MaxValue)]
    public int Pagina { get; set; } = 1;

    [Range(1, 100)]
    public int TamanoPagina { get; set; } = 25;
}
