using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiUsuariosRC.DBOperation.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaActualizacionAndPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Usuarios",
                type: "datetime2(0)",
                nullable: true);

            migrationBuilder.Sql(
                """
                CREATE OR ALTER PROCEDURE dbo.usp_BuscarUsuarios
                    @UsuarioId INT = NULL,
                    @NombreOCorreo NVARCHAR(150) = NULL,
                    @Activo BIT = NULL,
                    @FechaCreacion DATETIME2(0) = NULL,
                    @Pagina INT = 1,
                    @TamanoPagina INT = 25
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SET @Pagina = CASE WHEN @Pagina IS NULL OR @Pagina < 1 THEN 1 ELSE @Pagina END;
                    SET @TamanoPagina = CASE
                        WHEN @TamanoPagina IS NULL OR @TamanoPagina < 1 THEN 25
                        WHEN @TamanoPagina > 100 THEN 100
                        ELSE @TamanoPagina
                    END;

                    DECLARE @Offset INT = (@Pagina - 1) * @TamanoPagina;

                    ;WITH Filtrados AS
                    (
                        SELECT
                            u.UsuarioId,
                            u.NumeroEmpleado,
                            u.GeneroId,
                            g.Genero,
                            u.Nombres,
                            u.ApellidoPaterno,
                            u.ApellidoMaterno,
                            u.Correo,
                            u.Activo,
                            u.FechaNacimiento,
                            u.FechaCreacion,
                            u.FechaActualizacion,
                            COUNT(1) OVER() AS TotalRegistros
                        FROM dbo.Usuarios AS u
                        INNER JOIN dbo.Genero AS g ON g.GeneroId = u.GeneroId
                        WHERE (@UsuarioId IS NULL OR u.UsuarioId = @UsuarioId)
                          AND (@Activo IS NULL OR u.Activo = @Activo)
                          AND (
                                @NombreOCorreo IS NULL
                                OR LTRIM(RTRIM(@NombreOCorreo)) = ''
                                OR u.Nombres LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                                OR u.ApellidoPaterno LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                                OR u.ApellidoMaterno LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                                OR CONCAT(u.Nombres, ' ', u.ApellidoPaterno, ' ', u.ApellidoMaterno) LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                                OR u.Correo LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                              )
                          AND (
                                @FechaCreacion IS NULL
                                OR CAST(u.FechaCreacion AS DATE) = CAST(@FechaCreacion AS DATE)
                              )
                    )
                    SELECT
                        UsuarioId,
                        NumeroEmpleado,
                        GeneroId,
                        Genero,
                        Nombres,
                        ApellidoPaterno,
                        ApellidoMaterno,
                        Correo,
                        Activo,
                        FechaNacimiento,
                        FechaCreacion,
                        FechaActualizacion,
                        TotalRegistros
                    FROM Filtrados
                    ORDER BY UsuarioId
                    OFFSET @Offset ROWS FETCH NEXT @TamanoPagina ROWS ONLY;
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR ALTER PROCEDURE dbo.usp_BuscarUsuarios
                    @UsuarioId INT = NULL,
                    @NombreOCorreo NVARCHAR(150) = NULL,
                    @Activo BIT = NULL,
                    @FechaCreacion DATETIME2(0) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT
                        u.UsuarioId,
                        u.NumeroEmpleado,
                        u.GeneroId,
                        g.Genero,
                        u.Nombres,
                        u.ApellidoPaterno,
                        u.ApellidoMaterno,
                        u.Correo,
                        u.Activo,
                        u.FechaNacimiento,
                        u.FechaCreacion
                    FROM dbo.Usuarios AS u
                    INNER JOIN dbo.Genero AS g ON g.GeneroId = u.GeneroId
                    WHERE (@UsuarioId IS NULL OR u.UsuarioId = @UsuarioId)
                      AND (@Activo IS NULL OR u.Activo = @Activo)
                      AND (
                            @NombreOCorreo IS NULL
                            OR LTRIM(RTRIM(@NombreOCorreo)) = ''
                            OR u.Nombres LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                            OR u.Correo LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                          )
                      AND (
                            @FechaCreacion IS NULL
                            OR CAST(u.FechaCreacion AS DATE) = CAST(@FechaCreacion AS DATE)
                          )
                    ORDER BY u.UsuarioId;
                END;
                """);

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Usuarios");
        }
    }
}
