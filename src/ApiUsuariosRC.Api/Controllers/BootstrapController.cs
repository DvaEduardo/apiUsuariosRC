using ApiUsuariosRC.Api.Security;
using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsuariosRC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SkipActiveEmployeeValidation]
public class BootstrapController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public BootstrapController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Crea el primer usuario activo cuando la tabla Usuarios esta vacia.
    /// </summary>
    [HttpPost("usuario-inicial")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> CreateInitialUser(
        [FromBody] CrearUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _usuarioService.CreateInitialAsync(request, cancellationToken);
            return CreatedAtAction("GetById", "Usuarios", new { usuarioId = usuario.UsuarioId }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Bootstrap no disponible",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}
