using System.Net;

namespace ApiUsuariosRC.IntegrationTests;

public class HealthEndpointTests : IClassFixture<ApiUsuariosRcFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(ApiUsuariosRcFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
