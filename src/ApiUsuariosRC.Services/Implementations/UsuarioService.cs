using ApiUsuariosRC.DBOperation.Repositories;
using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Entities.Models;
using ApiUsuariosRC.Services.EmployeeNumbers;
using ApiUsuariosRC.Services.Interfaces;
using ApiUsuariosRC.Services.Time;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ApiUsuariosRC.Services.Implementations;

public class UsuarioService : IUsuarioService
{
    private static readonly string[] NombresBase =
    [
        "Laura", "Carlos", "Ana", "Jose", "Mariana", "Luis", "Fernanda", "Jorge"
    ];

    private static readonly string[] ApellidosBase =
    [
        "Ramirez", "Hernandez", "Santos", "Lopez", "Garcia", "Torres", "Morales", "Vega"
    ];

    private readonly IMapper _mapper;
    private readonly IGeneroRepository _generoRepository;
    private readonly IMexicoTimeService _mexicoTimeService;
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(
        IMapper mapper,
        IGeneroRepository generoRepository,
        IMexicoTimeService mexicoTimeService,
        IUsuarioRepository usuarioRepository)
    {
        _mapper = mapper;
        _generoRepository = generoRepository;
        _mexicoTimeService = mexicoTimeService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto> CreateAsync(CrearUsuarioRequestDto request, CancellationToken cancellationToken = default)
    {
        return await CreateInternalAsync(request, cancellationToken);
    }

    public async Task<UsuarioDto> CreateInitialAsync(CrearUsuarioRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await _usuarioRepository.AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("El usuario inicial solo puede crearse cuando la tabla Usuarios esta vacia.");
        }

        return await CreateInternalAsync(request, cancellationToken);
    }

    private async Task<UsuarioDto> CreateInternalAsync(CrearUsuarioRequestDto request, CancellationToken cancellationToken)
    {
        var correo = NormalizeEmail(request.Correo);
        ValidateFechaNacimiento(request.FechaNacimiento);
        var genero = await GetRequiredGeneroAsync(request.GeneroId, cancellationToken);

        await ValidateUniqueEmailAsync(correo, null, cancellationToken);

        var usuario = _mapper.Map<Usuario>(request);
        usuario.Genero = genero;
        usuario.Nombres = NormalizeText(usuario.Nombres);
        usuario.ApellidoPaterno = NormalizeText(usuario.ApellidoPaterno);
        usuario.ApellidoMaterno = NormalizeText(usuario.ApellidoMaterno);
        usuario.Correo = correo;
        usuario.FechaNacimiento = usuario.FechaNacimiento.Date;
        usuario.FechaCreacion = _mexicoTimeService.GetCurrentMexicoCityDateTime();
        usuario.NumeroEmpleado = await GenerateUniqueEmployeeNumberAsync(
            usuario,
            genero.Nombre,
            usuario.FechaCreacion.Year,
            null,
            null,
            cancellationToken);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _usuarioRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<IReadOnlyCollection<UsuarioDto>> GenerateAsync(GenerarUsuariosRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Cantidad <= 0)
        {
            throw new InvalidOperationException("La cantidad de registros a generar debe ser mayor a cero.");
        }

        var usuarios = new List<Usuario>(request.Cantidad);
        var reservedEmployeeNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var generos = await GetGenerosForGenerationAsync(request.GeneroId, cancellationToken);
        var maxUsuarioId = await _usuarioRepository.GetMaxUsuarioIdAsync(cancellationToken);
        var fechaCreacion = _mexicoTimeService.GetCurrentMexicoCityDateTime();

        for (var index = 1; index <= request.Cantidad; index++)
        {
            var secuencia = maxUsuarioId + index;
            var nombres = NombresBase[(secuencia - 1) % NombresBase.Length];
            var apellidoPaterno = ApellidosBase[(secuencia - 1) % ApellidosBase.Length];
            var apellidoMaterno = ApellidosBase[secuencia % ApellidosBase.Length];
            var genero = generos[(index - 1) % generos.Count];

            var usuario = new Usuario
            {
                GeneroId = genero.GeneroId,
                Nombres = nombres,
                ApellidoPaterno = apellidoPaterno,
                ApellidoMaterno = apellidoMaterno,
                Correo = $"{nombres}.{apellidoPaterno}.{secuencia}@demo.com".ToLowerInvariant(),
                Activo = true,
                FechaNacimiento = BuildGeneratedBirthDate(secuencia),
                FechaCreacion = fechaCreacion
            };

            usuario.NumeroEmpleado = await GenerateUniqueEmployeeNumberAsync(
                usuario,
                genero.Nombre,
                fechaCreacion.Year,
                reservedEmployeeNumbers,
                null,
                cancellationToken);

            usuarios.Add(usuario);
        }

        await _usuarioRepository.AddRangeAsync(usuarios, cancellationToken);
        await _usuarioRepository.SaveChangesAsync(cancellationToken);

        var generoLookup = generos.ToDictionary(x => x.GeneroId);
        foreach (var usuario in usuarios)
        {
            usuario.Genero = generoLookup[usuario.GeneroId];
        }

        return _mapper.Map<IReadOnlyCollection<UsuarioDto>>(usuarios);
    }

    public async Task<PagedResultDto<UsuarioDto>> GetAllAsync(PaginacionRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = _usuarioRepository
            .Query()
            .OrderBy(x => x.UsuarioId);

        var totalRegistros = await query.CountAsync(cancellationToken);
        var usuarios = await query
            .Skip(CalculateSkip(request.Pagina, request.TamanoPagina))
            .Take(request.TamanoPagina)
            .ProjectTo<UsuarioDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return PagedResultDto<UsuarioDto>.Create(usuarios, request.Pagina, request.TamanoPagina, totalRegistros);
    }

    public async Task<UsuarioDto?> GetByIdAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        return await _usuarioRepository
            .Query()
            .Where(x => x.UsuarioId == usuarioId)
            .ProjectTo<UsuarioDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResultDto<UsuarioConHistorialDto>> SearchAsync(BuscarUsuariosRequestDto request, CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.SearchAsync(
            request.UsuarioId,
            NormalizeOptionalText(request.NombreOCorreo),
            request.Activo,
            request.FechaCreacion,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken);

        var totalRegistros = usuarios.FirstOrDefault()?.TotalRegistros ?? 0;

        var logs = await _usuarioRepository.GetLogsByUsuarioIdsAsync(
            usuarios.Select(x => x.UsuarioId),
            cancellationToken);

        var logsByUsuario = logs
            .GroupBy(x => x.UsuarioIdAfectado)
            .ToDictionary(
                x => x.Key,
                x => _mapper.Map<IReadOnlyCollection<UsuarioCambioLogDto>>(x.ToList()));

        var response = _mapper.Map<List<UsuarioConHistorialDto>>(usuarios);
        foreach (var usuario in response)
        {
            usuario.HistorialCambios = logsByUsuario.TryGetValue(usuario.UsuarioId, out var historial)
                ? historial
                : [];
        }

        return PagedResultDto<UsuarioConHistorialDto>.Create(response, request.Pagina, request.TamanoPagina, totalRegistros);
    }

    public async Task<UsuarioDto?> UpdateAsync(int usuarioId, ActualizarUsuarioRequestDto request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken);
        if (usuario is null)
        {
            return null;
        }

        var nuevoCorreo = NormalizeEmail(request.Correo);
        var nuevaFechaNacimiento = request.FechaNacimiento.Date;
        ValidateFechaNacimiento(nuevaFechaNacimiento);
        var genero = await GetRequiredGeneroAsync(request.GeneroId, cancellationToken);
        await ValidateUniqueEmailAsync(nuevoCorreo, usuarioId, cancellationToken);

        var usuarioPropuesto = new Usuario
        {
            UsuarioId = usuario.UsuarioId,
            GeneroId = genero.GeneroId,
            Nombres = NormalizeText(request.Nombres),
            ApellidoPaterno = NormalizeText(request.ApellidoPaterno),
            ApellidoMaterno = NormalizeText(request.ApellidoMaterno),
            Correo = nuevoCorreo,
            Activo = request.Activo,
            FechaNacimiento = nuevaFechaNacimiento,
            FechaCreacion = usuario.FechaCreacion
        };

        var nuevoNumeroEmpleado = await GenerateUniqueEmployeeNumberAsync(
            usuarioPropuesto,
            genero.Nombre,
            usuario.FechaCreacion.Year,
            null,
            usuarioId,
            cancellationToken);

        var fechaOperacion = _mexicoTimeService.GetCurrentMexicoCityDateTimeOffset();
        var logs = BuildUpdateLogs(usuario, request, genero, nuevoNumeroEmpleado, fechaOperacion);

        usuario.GeneroId = genero.GeneroId;
        usuario.Genero = genero;
        usuario.NumeroEmpleado = nuevoNumeroEmpleado;
        usuario.Nombres = usuarioPropuesto.Nombres;
        usuario.ApellidoPaterno = usuarioPropuesto.ApellidoPaterno;
        usuario.ApellidoMaterno = usuarioPropuesto.ApellidoMaterno;
        usuario.Correo = nuevoCorreo;
        usuario.Activo = request.Activo;
        usuario.FechaNacimiento = nuevaFechaNacimiento;
        if (logs.Count > 0)
        {
            usuario.FechaActualizacion = fechaOperacion.DateTime;
        }

        _usuarioRepository.Update(usuario);
        if (logs.Count > 0)
        {
            await _usuarioRepository.AddLogsAsync(logs, cancellationToken);
        }

        await _usuarioRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto?> DeactivateAsync(int usuarioId, BajaUsuarioRequestDto request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken);
        if (usuario is null)
        {
            return null;
        }

        if (!usuario.Activo)
        {
            throw new InvalidOperationException("El usuario ya se encuentra inactivo.");
        }

        var fechaOperacion = _mexicoTimeService.GetCurrentMexicoCityDateTimeOffset();
        usuario.Activo = false;
        usuario.FechaActualizacion = fechaOperacion.DateTime;

        var log = new UsuarioCambioLog
        {
            UsuarioIdAfectado = usuario.UsuarioId,
            UsuarioIdAccion = request.UsuarioAccionId,
            TipoOperacion = "BajaLogica",
            CampoModificado = "Activo",
            ValorAnterior = true.ToString(),
            ValorNuevo = false.ToString(),
            FechaOperacion = fechaOperacion
        };

        _usuarioRepository.Update(usuario);
        await _usuarioRepository.AddLogsAsync([log], cancellationToken);
        await _usuarioRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UsuarioDto>(usuario);
    }

    private List<UsuarioCambioLog> BuildUpdateLogs(
        Usuario usuarioActual,
        ActualizarUsuarioRequestDto request,
        Genero genero,
        string nuevoNumeroEmpleado,
        DateTimeOffset fechaOperacion)
    {
        var logs = new List<UsuarioCambioLog>();
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "NumeroEmpleado", usuarioActual.NumeroEmpleado, nuevoNumeroEmpleado, fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "GeneroId", usuarioActual.GeneroId.ToString(CultureInfo.InvariantCulture), genero.GeneroId.ToString(CultureInfo.InvariantCulture), fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "Genero", usuarioActual.Genero?.Nombre ?? string.Empty, genero.Nombre, fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "Nombres", usuarioActual.Nombres, NormalizeText(request.Nombres), fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "ApellidoPaterno", usuarioActual.ApellidoPaterno, NormalizeText(request.ApellidoPaterno), fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "ApellidoMaterno", usuarioActual.ApellidoMaterno, NormalizeText(request.ApellidoMaterno), fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "Correo", usuarioActual.Correo, NormalizeEmail(request.Correo), fechaOperacion);
        RegisterLogIfChanged(logs, usuarioActual.UsuarioId, request.UsuarioAccionId, "Activo", usuarioActual.Activo.ToString(), request.Activo.ToString(), fechaOperacion);
        RegisterLogIfChanged(
            logs,
            usuarioActual.UsuarioId,
            request.UsuarioAccionId,
            "FechaNacimiento",
            FormatDate(usuarioActual.FechaNacimiento),
            FormatDate(request.FechaNacimiento.Date),
            fechaOperacion);
        return logs;
    }

    private static void RegisterLogIfChanged(
        ICollection<UsuarioCambioLog> logs,
        int usuarioIdAfectado,
        int usuarioIdAccion,
        string campo,
        string valorActual,
        string nuevoValor,
        DateTimeOffset fechaOperacion)
    {
        if (string.Equals(valorActual, nuevoValor, StringComparison.Ordinal))
        {
            return;
        }

        logs.Add(new UsuarioCambioLog
        {
            UsuarioIdAfectado = usuarioIdAfectado,
            UsuarioIdAccion = usuarioIdAccion,
            TipoOperacion = "Actualizacion",
            CampoModificado = campo,
            ValorAnterior = valorActual,
            ValorNuevo = nuevoValor,
            FechaOperacion = fechaOperacion
        });
    }

    private async Task ValidateUniqueEmailAsync(
        string correo,
        int? excludeUsuarioId,
        CancellationToken cancellationToken)
    {
        if (await _usuarioRepository.ExistsCorreoAsync(correo, excludeUsuarioId, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.");
        }
    }

    private async Task<string> GenerateUniqueEmployeeNumberAsync(
        Usuario usuario,
        string genero,
        int registrationYear,
        ISet<string>? reservedEmployeeNumbers,
        int? excludeUsuarioId,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 1296;

        var baseNumber = EmployeeNumberGenerator.Generate(usuario, genero, registrationYear);
        var candidate = baseNumber;
        var collisionIndex = 0;

        while (await EmployeeNumberExistsAsync(candidate, reservedEmployeeNumbers, excludeUsuarioId, cancellationToken))
        {
            collisionIndex++;
            if (collisionIndex > maxAttempts)
            {
                throw new InvalidOperationException("No fue posible generar un numero de empleado unico.");
            }

            candidate = EmployeeNumberGenerator.WithCollisionSuffix(baseNumber, collisionIndex);
        }

        reservedEmployeeNumbers?.Add(candidate);
        return candidate;
    }

    private async Task<bool> EmployeeNumberExistsAsync(
        string candidate,
        ISet<string>? reservedEmployeeNumbers,
        int? excludeUsuarioId,
        CancellationToken cancellationToken)
    {
        return (reservedEmployeeNumbers?.Contains(candidate) ?? false)
            || await _usuarioRepository.ExistsNumeroEmpleadoAsync(candidate, excludeUsuarioId, cancellationToken);
    }

    private async Task<Genero> GetRequiredGeneroAsync(int generoId, CancellationToken cancellationToken)
    {
        var genero = await _generoRepository.GetByIdAsync(generoId, cancellationToken);
        if (genero is null)
        {
            throw new InvalidOperationException("El genero indicado no existe.");
        }

        return genero;
    }

    private async Task<IReadOnlyList<Genero>> GetGenerosForGenerationAsync(int? generoId, CancellationToken cancellationToken)
    {
        if (generoId.HasValue)
        {
            return [await GetRequiredGeneroAsync(generoId.Value, cancellationToken)];
        }

        var generos = await _generoRepository.GetAllAsync(cancellationToken);
        if (generos.Count == 0)
        {
            throw new InvalidOperationException("Debe existir al menos un genero para generar usuarios.");
        }

        return generos.ToList();
    }

    private static DateTime BuildGeneratedBirthDate(int sequence)
    {
        var year = 1980 + (sequence % 25);
        var month = (sequence % 12) + 1;
        var day = (sequence % 28) + 1;

        return new DateTime(year, month, day);
    }

    private static int CalculateSkip(int pagina, int tamanoPagina)
    {
        return (pagina - 1) * tamanoPagina;
    }

    private void ValidateFechaNacimiento(DateTime fechaNacimiento)
    {
        var today = _mexicoTimeService.GetCurrentMexicoCityDateTime().Date;
        var normalizedDate = fechaNacimiento.Date;

        if (normalizedDate < new DateTime(1900, 1, 1) || normalizedDate >= today)
        {
            throw new InvalidOperationException("La fecha de nacimiento debe ser una fecha valida anterior al dia actual.");
        }
    }

    private static string NormalizeEmail(string correo)
    {
        return correo.Trim().ToLowerInvariant();
    }

    private static string NormalizeText(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string FormatDate(DateTime value)
    {
        return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
