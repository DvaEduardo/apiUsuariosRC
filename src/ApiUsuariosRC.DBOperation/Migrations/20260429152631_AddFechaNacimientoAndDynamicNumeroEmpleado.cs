using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiUsuariosRC.DBOperation.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaNacimientoAndDynamicNumeroEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "datetime2(0)",
                nullable: false,
                defaultValue: new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                        UsuarioId,
                        NumeroEmpleado,
                        Nombres,
                        ApellidoPaterno,
                        ApellidoMaterno,
                        Correo,
                        Activo,
                        FechaNacimiento,
                        FechaCreacion
                    FROM dbo.Usuarios
                    WHERE (@UsuarioId IS NULL OR UsuarioId = @UsuarioId)
                      AND (@Activo IS NULL OR Activo = @Activo)
                      AND (
                            @NombreOCorreo IS NULL
                            OR LTRIM(RTRIM(@NombreOCorreo)) = ''
                            OR Nombres LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                            OR Correo LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                          )
                      AND (
                            @FechaCreacion IS NULL
                            OR CAST(FechaCreacion AS DATE) = CAST(@FechaCreacion AS DATE)
                          )
                    ORDER BY UsuarioId;
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
                        UsuarioId,
                        NumeroEmpleado,
                        Nombres,
                        ApellidoPaterno,
                        ApellidoMaterno,
                        Correo,
                        Activo,
                        FechaCreacion
                    FROM dbo.Usuarios
                    WHERE (@UsuarioId IS NULL OR UsuarioId = @UsuarioId)
                      AND (@Activo IS NULL OR Activo = @Activo)
                      AND (
                            @NombreOCorreo IS NULL
                            OR LTRIM(RTRIM(@NombreOCorreo)) = ''
                            OR Nombres LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                            OR Correo LIKE '%' + LTRIM(RTRIM(@NombreOCorreo)) + '%'
                          )
                      AND (
                            @FechaCreacion IS NULL
                            OR CAST(FechaCreacion AS DATE) = CAST(@FechaCreacion AS DATE)
                          )
                    ORDER BY UsuarioId;
                END;
                """);

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Usuarios");
        }
    }
}
