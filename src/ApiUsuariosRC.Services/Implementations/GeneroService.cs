using ApiUsuariosRC.DBOperation.Repositories;
using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Entities.Models;
using ApiUsuariosRC.Services.Interfaces;
using ApiUsuariosRC.Services.Time;
using AutoMapper;

namespace ApiUsuariosRC.Services.Implementations;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _generoRepository;
    private readonly IMapper _mapper;
    private readonly IMexicoTimeService _mexicoTimeService;

    public GeneroService(
        IGeneroRepository generoRepository,
        IMapper mapper,
        IMexicoTimeService mexicoTimeService)
    {
        _generoRepository = generoRepository;
        _mapper = mapper;
        _mexicoTimeService = mexicoTimeService;
    }

    public async Task<IReadOnlyCollection<GeneroDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var generos = await _generoRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<GeneroDto>>(generos);
    }

    public async Task<GeneroDto?> GetByIdAsync(int generoId, CancellationToken cancellationToken = default)
    {
        var genero = await _generoRepository.GetByIdAsync(generoId, cancellationToken);
        return genero is null ? null : _mapper.Map<GeneroDto>(genero);
    }

    public async Task<GeneroDto> CreateAsync(CrearGeneroRequestDto request, CancellationToken cancellationToken = default)
    {
        var generoNombre = NormalizeGenero(request.Genero);
        await ValidateUniqueGeneroAsync(generoNombre, null, cancellationToken);

        var genero = new Genero
        {
            Nombre = generoNombre,
            FechaCreacion = _mexicoTimeService.GetCurrentMexicoCityDateTime()
        };

        await _generoRepository.AddAsync(genero, cancellationToken);
        await _generoRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GeneroDto>(genero);
    }

    public async Task<GeneroDto?> UpdateAsync(int generoId, ActualizarGeneroRequestDto request, CancellationToken cancellationToken = default)
    {
        var genero = await _generoRepository.GetByIdAsync(generoId, cancellationToken);
        if (genero is null)
        {
            return null;
        }

        var generoNombre = NormalizeGenero(request.Genero);
        await ValidateUniqueGeneroAsync(generoNombre, generoId, cancellationToken);

        genero.Nombre = generoNombre;
        _generoRepository.Update(genero);
        await _generoRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GeneroDto>(genero);
    }

    public async Task<bool> DeleteAsync(int generoId, CancellationToken cancellationToken = default)
    {
        var genero = await _generoRepository.GetByIdAsync(generoId, cancellationToken);
        if (genero is null)
        {
            return false;
        }

        if (await _generoRepository.HasUsersAsync(generoId, cancellationToken))
        {
            throw new InvalidOperationException("No se puede eliminar un genero que tiene usuarios asociados.");
        }

        _generoRepository.Remove(genero);
        await _generoRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task ValidateUniqueGeneroAsync(string genero, int? excludeGeneroId, CancellationToken cancellationToken)
    {
        if (await _generoRepository.ExistsByNameAsync(genero, excludeGeneroId, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un genero con ese nombre.");
        }
    }

    private static string NormalizeGenero(string genero)
    {
        return genero.Trim();
    }
}
