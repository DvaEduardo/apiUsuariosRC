using ApiUsuariosRC.Entities.Dtos;

namespace ApiUsuariosRC.Services.Interfaces;

public interface IGeneroService
{
    Task<IReadOnlyCollection<GeneroDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<GeneroDto?> GetByIdAsync(int generoId, CancellationToken cancellationToken = default);

    Task<GeneroDto> CreateAsync(CrearGeneroRequestDto request, CancellationToken cancellationToken = default);

    Task<GeneroDto?> UpdateAsync(int generoId, ActualizarGeneroRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int generoId, CancellationToken cancellationToken = default);
}
