using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsuariosRC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Create(
        [FromBody] CrearUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _usuarioService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { usuarioId = usuario.UsuarioId }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BuildProblemDetails(ex.Message));
        }
    }

    [HttpPost("generar")]
    [ProducesResponseType(typeof(IReadOnlyCollection<UsuarioDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<IReadOnlyCollection<UsuarioDto>>> Generate(
        [FromBody] GenerarUsuariosRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarios = await _usuarioService.GenerateAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, usuarios);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BuildProblemDetails(ex.Message));
        }
    }

    /// <summary>
    /// Obtiene usuarios paginados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<UsuarioDto>>> GetAll(
        [FromQuery] PaginacionRequestDto request,
        CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.GetAllAsync(request, cancellationToken);
        return Ok(usuarios);
    }

    [HttpGet("{usuarioId:int}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioDto>> GetById(int usuarioId, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.GetByIdAsync(usuarioId, cancellationToken);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost("buscar")]
    [ProducesResponseType(typeof(PagedResultDto<UsuarioConHistorialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<UsuarioConHistorialDto>>> Search(
        [FromBody] BuscarUsuariosRequestDto request,
        CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.SearchAsync(request, cancellationToken);
        return Ok(usuarios);
    }

    [HttpPut("{usuarioId:int}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Update(
        int usuarioId,
        [FromBody] ActualizarUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _usuarioService.UpdateAsync(usuarioId, request, cancellationToken);
            return usuario is null ? NotFound() : Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BuildProblemDetails(ex.Message));
        }
    }

    [HttpPost("{usuarioId:int}/baja")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Deactivate(
        int usuarioId,
        [FromBody] BajaUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _usuarioService.DeactivateAsync(usuarioId, request, cancellationToken);
            return usuario is null ? NotFound() : Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BuildProblemDetails(ex.Message));
        }
    }

    private static ProblemDetails BuildProblemDetails(string detail)
    {
        return new ProblemDetails
        {
            Title = "Conflicto de negocio",
            Detail = detail,
            Status = StatusCodes.Status409Conflict
        };
    }
}
