using ApiUsuariosRC.Entities.Models;

namespace ApiUsuariosRC.DBOperation.Repositories;

public interface IGeneroRepository
{
    IQueryable<Genero> Query();

    Task<IReadOnlyCollection<Genero>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Genero?> GetByIdAsync(int generoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string genero, int? excludeGeneroId = null, CancellationToken cancellationToken = default);

    Task<bool> HasUsersAsync(int generoId, CancellationToken cancellationToken = default);

    Task AddAsync(Genero genero, CancellationToken cancellationToken = default);

    void Update(Genero genero);

    void Remove(Genero genero);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
