using System.Net;
using System.Net.Http.Json;
using ApiUsuariosRC.Entities.Dtos;

namespace ApiUsuariosRC.IntegrationTests;

public class UsuariosEndpointTests : IClassFixture<ApiUsuariosRcFactory>
{
    private readonly HttpClient _client;

    public UsuariosEndpointTests(ApiUsuariosRcFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsuarios_WithoutNumeroEmpleado_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/usuarios");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUsuarios_WithInvalidNumeroEmpleado_ReturnsForbidden()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/usuarios");
        request.Headers.Add("X-Numero-Empleado", "RC-NO-EXISTE");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsuarios_WithActiveNumeroEmpleado_ReturnsPagedUsers()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/usuarios?pagina=1&tamanoPagina=10");
        request.Headers.Add("X-Numero-Empleado", ApiUsuariosRcFactory.ActiveNumeroEmpleado);

        var response = await _client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<UsuarioDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRegistros);
        Assert.Single(result.Items);
        Assert.Equal(ApiUsuariosRcFactory.ActiveNumeroEmpleado, result.Items.First().NumeroEmpleado);
    }

    [Fact]
    public async Task Preflight_FromLocalFrontend_ReturnsCorsHeaders()
    {
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/usuarios");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "X-Numero-Empleado");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins));
        Assert.Contains("http://localhost:5173", origins);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Headers", out var allowedHeaders));
        Assert.Contains("X-Numero-Empleado", string.Join(',', allowedHeaders));
    }

    [Fact]
    public async Task Bootstrap_WhenUsersExist_ReturnsConflictWithoutHeader()
    {
        var response = await _client.PostAsJsonAsync("/api/bootstrap/usuario-inicial", new CrearUsuarioRequestDto
        {
            GeneroId = 1,
            Nombres = "Nuevo",
            ApellidoPaterno = "Usuario",
            ApellidoMaterno = "Inicial",
            Correo = "nuevo.inicial@example.com",
            Activo = true,
            FechaNacimiento = new DateTime(1995, 1, 1)
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
