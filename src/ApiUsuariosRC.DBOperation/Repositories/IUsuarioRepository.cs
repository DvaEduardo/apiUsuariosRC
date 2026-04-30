using ApiUsuariosRC.Entities.Models;

namespace ApiUsuariosRC.DBOperation.Repositories;

public interface IUsuarioRepository
{
    IQueryable<Usuario> Query();

    Task<Usuario?> GetByIdAsync(int usuarioId, CancellationToken cancellationToken = default);

    Task<Usuario?> GetActiveByNumeroEmpleadoAsync(string numeroEmpleado, CancellationToken cancellationToken = default);

    Task<bool> ExistsNumeroEmpleadoAsync(string numeroEmpleado, int? excludeUsuarioId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsCorreoAsync(string correo, int? excludeUsuarioId = null, CancellationToken cancellationToken = default);

    Task<int> GetMaxUsuarioIdAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UsuarioConsulta>> SearchAsync(
        int? usuarioId,
        string? nombreOCorreo,
        bool? activo,
        DateTime? fechaCreacion,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UsuarioCambioLog>> GetLogsByUsuarioIdsAsync(
        IEnumerable<int> usuarioIds,
        CancellationToken cancellationToken = default);

    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<Usuario> usuarios, CancellationToken cancellationToken = default);

    Task AddLogsAsync(IEnumerable<UsuarioCambioLog> logs, CancellationToken cancellationToken = default);

    void Update(Usuario usuario);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
