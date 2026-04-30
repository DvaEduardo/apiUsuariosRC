using ApiUsuariosRC.Entities.Dtos;

namespace ApiUsuariosRC.Services.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto> CreateAsync(CrearUsuarioRequestDto request, CancellationToken cancellationToken = default);

    Task<UsuarioDto> CreateInitialAsync(CrearUsuarioRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UsuarioDto>> GenerateAsync(GenerarUsuariosRequestDto request, CancellationToken cancellationToken = default);

    Task<PagedResultDto<UsuarioDto>> GetAllAsync(PaginacionRequestDto request, CancellationToken cancellationToken = default);

    Task<UsuarioDto?> GetByIdAsync(int usuarioId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<UsuarioConHistorialDto>> SearchAsync(BuscarUsuariosRequestDto request, CancellationToken cancellationToken = default);

    Task<UsuarioDto?> UpdateAsync(int usuarioId, ActualizarUsuarioRequestDto request, CancellationToken cancellationToken = default);

    Task<UsuarioDto?> DeactivateAsync(int usuarioId, BajaUsuarioRequestDto request, CancellationToken cancellationToken = default);
}
