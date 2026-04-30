using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsuariosRC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerosController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GenerosController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<GeneroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<GeneroDto>>> GetAll(CancellationToken cancellationToken)
    {
        var generos = await _generoService.GetAllAsync(cancellationToken);
        return Ok(generos);
    }

    [HttpGet("{generoId:int}")]
    [ProducesResponseType(typeof(GeneroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GeneroDto>> GetById(int generoId, CancellationToken cancellationToken)
    {
        var genero = await _generoService.GetByIdAsync(generoId, cancellationToken);
        return genero is null ? NotFound() : Ok(genero);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GeneroDto>> Create(
        [FromBody] CrearGeneroRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var genero = await _generoService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { generoId = genero.GeneroId }, genero);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BuildProblemDetails(ex.Message));
        }
    }

    [HttpPut("{generoId:int}")]
    [ProducesResponseType(typeof(GeneroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GeneroDto>> Update(
        int generoId,
        [FromBody] ActualizarGeneroRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var genero = await _generoService.UpdateAsync(generoId, request, cancellationToken);
            return genero is null ? NotFound() : Ok(genero);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BuildProblemDetails(ex.Message));
        }
    }

    [HttpDelete("{generoId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int generoId, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _generoService.DeleteAsync(generoId, cancellationToken);
            return deleted ? NoContent() : NotFound();
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
