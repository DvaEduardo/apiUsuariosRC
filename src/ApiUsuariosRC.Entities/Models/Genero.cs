namespace ApiUsuariosRC.Entities.Models;

public class Genero
{
    public int GeneroId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = [];
}
