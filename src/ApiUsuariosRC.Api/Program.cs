using ApiUsuariosRC.DBOperation.Context;
using ApiUsuariosRC.DBOperation.Repositories;
using ApiUsuariosRC.Api.Filters;
using ApiUsuariosRC.Api.Swagger;
using ApiUsuariosRC.Services.Implementations;
using ApiUsuariosRC.Services.Interfaces;
using ApiUsuariosRC.Services.Mappings;
using ApiUsuariosRC.Services.Time;
using ApiUsuariosRC.Services.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

const string FrontendCorsPolicy = "FrontendLocalhostCorsPolicy";

var builder = WebApplication.CreateBuilder(args);
var allowedCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(x => x.Value)
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => x!)
    .ToArray();

if (allowedCorsOrigins.Length == 0)
{
    allowedCorsOrigins = ["http://localhost:5173"];
}

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ActiveEmployeeFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CrearUsuarioRequestDtoValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiUsuariosRC",
        Version = "v1",
        Description = "API en capas con ASP.NET Core 8, SQL Server, Entity Framework Core, AutoMapper y auditoria de cambios."
    });
    options.OperationFilter<NumeroEmpleadoHeaderOperationFilter>();
    options.SchemaFilter<RequestExampleSchemaFilter>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No se encontro la cadena de conexion 'DefaultConnection'. Configure User Secrets o una variable de entorno.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure();
    }));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IGeneroRepository, GeneroRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<IEmployeeRequestValidator, EmployeeRequestValidator>();
builder.Services.AddSingleton<IMexicoTimeService, MexicoTimeService>();
builder.Services.AddAutoMapper(cfg =>
{
    var licenseKey = builder.Configuration["AutoMapper:LicenseKey"];
    if (!string.IsNullOrWhiteSpace(licenseKey))
    {
        cfg.LicenseKey = licenseKey;
    }
}, typeof(UsuarioProfile).Assembly);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("sqlserver");

var app = builder.Build();
var swaggerEnabled = app.Configuration.GetValue("Swagger:Enabled", true);

app.UseExceptionHandler();

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program
{
}
