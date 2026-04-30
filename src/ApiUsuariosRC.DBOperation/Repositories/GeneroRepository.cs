using ApiUsuariosRC.DBOperation.Context;
using ApiUsuariosRC.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuariosRC.DBOperation.Repositories;

public class GeneroRepository : IGeneroRepository
{
    private readonly AppDbContext _context;

    public GeneroRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Genero> Query()
    {
        return _context.Generos.AsNoTracking();
    }

    public async Task<IReadOnlyCollection<Genero>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Generos
            .AsNoTracking()
            .OrderBy(x => x.GeneroId)
            .ToListAsync(cancellationToken);
    }

    public Task<Genero?> GetByIdAsync(int generoId, CancellationToken cancellationToken = default)
    {
        return _context.Generos.FirstOrDefaultAsync(x => x.GeneroId == generoId, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string genero, int? excludeGeneroId = null, CancellationToken cancellationToken = default)
    {
        return _context.Generos
            .AsNoTracking()
            .AnyAsync(
                x => x.Nombre == genero && (!excludeGeneroId.HasValue || x.GeneroId != excludeGeneroId.Value),
                cancellationToken);
    }

    public Task<bool> HasUsersAsync(int generoId, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.AsNoTracking().AnyAsync(x => x.GeneroId == generoId, cancellationToken);
    }

    public Task AddAsync(Genero genero, CancellationToken cancellationToken = default)
    {
        return _context.Generos.AddAsync(genero, cancellationToken).AsTask();
    }

    public void Update(Genero genero)
    {
        _context.Generos.Update(genero);
    }

    public void Remove(Genero genero)
    {
        _context.Generos.Remove(genero);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
