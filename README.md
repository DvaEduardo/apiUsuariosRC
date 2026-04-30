# ApiUsuariosRC

API REST en .NET 8 para gestion de empleados. Incluye arquitectura en capas, Entity Framework Core con SQL Server, AutoMapper, Swagger, validaciones, auditoria de cambios, paginacion y endpoint de bootstrap para crear el primer usuario.

## Estructura

- `ApiUsuariosRC.Api`: controladores, Swagger, filtros y health checks.
- `ApiUsuariosRC.Services`: logica de negocio, AutoMapper, FluentValidation, seguridad por empleado activo y generador de numero de empleado.
- `ApiUsuariosRC.DBOperation`: `DbContext`, repositorios, migraciones y procedimiento almacenado.
- `ApiUsuariosRC.Entities`: entidades y DTOs.

## Requisitos

- .NET SDK 8.0.
- SQL Server o Azure SQL.
- Herramienta local `dotnet-ef`, incluida en `.config/dotnet-tools.json`.

## Configuracion

La cadena de conexion real no debe guardarse en `appsettings.json`. Para desarrollo local usa User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=SERVIDOR,1433;Initial Catalog=Data;Persist Security Info=True;User ID=USUARIO;Password=PASSWORD;" --project .\src\ApiUsuariosRC.Api\ApiUsuariosRC.Api.csproj
```

Tambien puedes usar variable de entorno:

```powershell
$env:ConnectionStrings__DefaultConnection="Data Source=SERVIDOR,1433;Initial Catalog=Data;Persist Security Info=True;User ID=USUARIO;Password=PASSWORD;"
```

## Ejecucion

```powershell
dotnet tool restore
dotnet restore
dotnet ef database update --project .\src\ApiUsuariosRC.DBOperation\ApiUsuariosRC.DBOperation.csproj --startup-project .\src\ApiUsuariosRC.Api\ApiUsuariosRC.Api.csproj --context AppDbContext
dotnet run --project .\src\ApiUsuariosRC.Api\ApiUsuariosRC.Api.csproj
```

URLs principales:

- Swagger: `https://localhost:7238/swagger`
- Health check: `https://localhost:7238/health`

## CORS

La API permite llamadas desde estos origenes:

```text
http://localhost:5173
http://redcompanies.tryasp.net
```

La configuracion esta en `Cors:AllowedOrigins` dentro de `appsettings.json`. La politica permite los headers necesarios para consumir endpoints protegidos, incluido `X-Numero-Empleado`.

## Seguridad actual

Todos los endpoints protegidos requieren el header:

```http
X-Numero-Empleado: RC-M-EDVIAM-8X96-26-5Q4V
```

La API valida que el numero exista y que el usuario este activo. Si falta el header responde `400`; si el numero no existe o esta inactivo responde `403`.

Para ambientes nuevos existe:

```http
POST /api/bootstrap/usuario-inicial
```

Este endpoint no requiere header, pero solo funciona si la tabla `Usuarios` esta vacia.

## Algoritmo de NumeroEmpleado

Formato:

```text
RC-{InicialGenero}-{HuellaNombre}-{NacimientoBase36}-{AnioRegistro}-{Checksum}
```

Reglas:

- `RC`: prefijo fijo del sistema.
- `InicialGenero`: primera letra normalizada del genero asociado. Ejemplo: `Masculino` genera `M`, `Femenino` genera `F`.
- `HuellaNombre`: 2 letras del primer nombre, 2 letras del apellido paterno y 2 letras del apellido materno. Se normalizan acentos y se convierten a mayusculas.
- `NacimientoBase36`: dia del anio de nacimiento convertido a base 36, mas los dos ultimos digitos del anio de nacimiento.
- `AnioRegistro`: dos ultimos digitos del anio en que se registra el empleado.
- `Checksum`: 4 caracteres base 36 generados con SHA256 a partir de genero, nombres, apellidos, fecha de nacimiento y anio de registro.
- Si existe colision, se agrega un sufijo incremental base 36: `-01`, `-02`, etc.

Ejemplo:

```text
RC-M-EDVIAM-8X96-26-5Q4V
```

Esto permite que el numero sea reproducible, legible, basado en datos del empleado y suficientemente robusto ante colisiones.

## Funcionalidad

- CRUD de usuarios.
- CRUD de generos.
- Generacion automatica de `NumeroEmpleado`.
- Campo `FechaCreacion` con hora de Ciudad de Mexico.
- Campo `FechaActualizacion` al editar o aplicar baja logica.
- Baja logica con `Activo = false`.
- Tabla de log `UsuarioCambiosLog` con usuario que realiza la operacion, campo modificado, valor anterior, valor nuevo y fecha exacta.
- Busqueda por procedimiento almacenado `dbo.usp_BuscarUsuarios`.
- Historial de cambios incluido en `POST /api/usuarios/buscar`.
- Paginacion en listado y busqueda.
- Validaciones con Data Annotations y FluentValidation.
- Health check de EF Core contra SQL Server.

## Pruebas

```powershell
dotnet test
```

Las pruebas unitarias cubren el generador de numero de empleado y validaciones. Las pruebas de integracion verifican el pipeline HTTP, seguridad por header y consulta paginada.

## Ejemplos

- Archivo HTTP: `docs\ApiUsuariosRC.http`
- Coleccion Postman: `docs\ApiUsuariosRC.postman_collection.json`
