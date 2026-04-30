namespace ApiUsuariosRC.Entities.Models;

public class UsuarioCambioLog
{
    public long LogId { get; set; }

    public int UsuarioIdAfectado { get; set; }

    public int UsuarioIdAccion { get; set; }

    public string TipoOperacion { get; set; } = string.Empty;

    public string CampoModificado { get; set; } = string.Empty;

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public DateTimeOffset FechaOperacion { get; set; }
}
