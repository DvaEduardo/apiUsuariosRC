using ApiUsuariosRC.DBOperation.Context;
using ApiUsuariosRC.Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiUsuariosRC.IntegrationTests;

public sealed class ApiUsuariosRcFactory : WebApplicationFactory<Program>
{
    public const string ActiveNumeroEmpleado = "RC-M-TESTUS-8X90-26-ABCD";

    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly string? _previousConnectionString;

    public ApiUsuariosRcFactory()
    {
        _previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Data Source=tests");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=tests"
            });
        });

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();

            Seed(context);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _previousConnectionString);
        _connection.Dispose();
    }

    private static void Seed(AppDbContext context)
    {
        if (context.Usuarios.Any())
        {
            return;
        }

        var genero = new Genero
        {
            GeneroId = 1,
            Nombre = "Masculino",
            FechaCreacion = new DateTime(2026, 4, 29, 10, 0, 0)
        };

        context.Generos.Add(genero);
        context.Usuarios.Add(new Usuario
        {
            UsuarioId = 1,
            NumeroEmpleado = ActiveNumeroEmpleado,
            GeneroId = genero.GeneroId,
            Nombres = "Test",
            ApellidoPaterno = "User",
            ApellidoMaterno = "Seed",
            Correo = "test.user@example.com",
            Activo = true,
            FechaNacimiento = new DateTime(1990, 11, 16),
            FechaCreacion = new DateTime(2026, 4, 29, 10, 0, 0)
        });

        context.SaveChanges();
    }
}
