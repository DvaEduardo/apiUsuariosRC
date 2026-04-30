using ApiUsuariosRC.DBOperation.Context;
using ApiUsuariosRC.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuariosRC.DBOperation.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Usuario> Query()
    {
        return _context.Usuarios.Include(x => x.Genero).AsNoTracking();
    }

    public Task<Usuario?> GetByIdAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.Include(x => x.Genero).FirstOrDefaultAsync(x => x.UsuarioId == usuarioId, cancellationToken);
    }

    public Task<Usuario?> GetActiveByNumeroEmpleadoAsync(string numeroEmpleado, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.NumeroEmpleado == numeroEmpleado && x.Activo,
                cancellationToken);
    }

    public Task<bool> ExistsNumeroEmpleadoAsync(string numeroEmpleado, int? excludeUsuarioId = null, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios
            .AsNoTracking()
            .AnyAsync(
                x => x.NumeroEmpleado == numeroEmpleado && (!excludeUsuarioId.HasValue || x.UsuarioId != excludeUsuarioId.Value),
                cancellationToken);
    }

    public Task<bool> ExistsCorreoAsync(string correo, int? excludeUsuarioId = null, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios
            .AsNoTracking()
            .AnyAsync(
                x => x.Correo == correo && (!excludeUsuarioId.HasValue || x.UsuarioId != excludeUsuarioId.Value),
                cancellationToken);
    }

    public async Task<int> GetMaxUsuarioIdAsync(CancellationToken cancellationToken = default)
    {
        var max = await _context.Usuarios
            .AsNoTracking()
            .Select(x => (int?)x.UsuarioId)
            .MaxAsync(cancellationToken);

        return max ?? 0;
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.AsNoTracking().AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UsuarioConsulta>> SearchAsync(
        int? usuarioId,
        string? nombreOCorreo,
        bool? activo,
        DateTime? fechaCreacion,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<UsuarioConsulta>()
            .FromSqlInterpolated(
                $@"EXEC dbo.usp_BuscarUsuarios
                    @UsuarioId={usuarioId},
                    @NombreOCorreo={nombreOCorreo},
                    @Activo={activo},
                    @FechaCreacion={fechaCreacion},
                    @Pagina={pagina},
                    @TamanoPagina={tamanoPagina}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UsuarioCambioLog>> GetLogsByUsuarioIdsAsync(
        IEnumerable<int> usuarioIds,
        CancellationToken cancellationToken = default)
    {
        var ids = usuarioIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return [];
        }

        return await _context.UsuarioCambiosLog
            .AsNoTracking()
            .Where(x => ids.Contains(x.UsuarioIdAfectado))
            .OrderByDescending(x => x.FechaOperacion)
            .ThenByDescending(x => x.LogId)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.AddAsync(usuario, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<Usuario> usuarios, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.AddRangeAsync(usuarios, cancellationToken);
    }

    public Task AddLogsAsync(IEnumerable<UsuarioCambioLog> logs, CancellationToken cancellationToken = default)
    {
        return _context.UsuarioCambiosLog.AddRangeAsync(logs, cancellationToken);
    }

    public void Update(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
