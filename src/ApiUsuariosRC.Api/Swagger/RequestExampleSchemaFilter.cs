using ApiUsuariosRC.Entities.Dtos;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiUsuariosRC.Api.Swagger;

public class RequestExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = context.Type.Name switch
        {
            nameof(CrearUsuarioRequestDto) => new OpenApiObject
            {
                ["generoId"] = new OpenApiInteger(1),
                ["nombres"] = new OpenApiString("Eduardo Daniel"),
                ["apellidoPaterno"] = new OpenApiString("Villanueva"),
                ["apellidoMaterno"] = new OpenApiString("Amaro"),
                ["correo"] = new OpenApiString("evillanueva@gmail.com"),
                ["activo"] = new OpenApiBoolean(true),
                ["fechaNacimiento"] = new OpenApiString("1996-11-16T00:00:00")
            },
            nameof(ActualizarUsuarioRequestDto) => new OpenApiObject
            {
                ["usuarioAccionId"] = new OpenApiInteger(1),
                ["generoId"] = new OpenApiInteger(1),
                ["nombres"] = new OpenApiString("Eduardo Daniel"),
                ["apellidoPaterno"] = new OpenApiString("Villanueva"),
                ["apellidoMaterno"] = new OpenApiString("Amaro"),
                ["correo"] = new OpenApiString("evillanueva@gmail.com"),
                ["activo"] = new OpenApiBoolean(true),
                ["fechaNacimiento"] = new OpenApiString("1996-11-16T00:00:00")
            },
            nameof(BuscarUsuariosRequestDto) => new OpenApiObject
            {
                ["usuarioId"] = new OpenApiNull(),
                ["nombreOCorreo"] = new OpenApiString("Eduardo"),
                ["activo"] = new OpenApiBoolean(true),
                ["fechaCreacion"] = new OpenApiNull(),
                ["pagina"] = new OpenApiInteger(1),
                ["tamanoPagina"] = new OpenApiInteger(25)
            },
            nameof(BajaUsuarioRequestDto) => new OpenApiObject
            {
                ["usuarioAccionId"] = new OpenApiInteger(1)
            },
            nameof(CrearGeneroRequestDto) => new OpenApiObject
            {
                ["genero"] = new OpenApiString("No binario")
            },
            nameof(GenerarUsuariosRequestDto) => new OpenApiObject
            {
                ["cantidad"] = new OpenApiInteger(5),
                ["generoId"] = new OpenApiInteger(1)
            },
            _ => schema.Example
        };
    }
}
