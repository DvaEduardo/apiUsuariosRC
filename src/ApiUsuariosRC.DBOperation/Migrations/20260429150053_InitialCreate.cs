using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiUsuariosRC.DBOperation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioCambiosLog",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioIdAfectado = table.Column<int>(type: "int", nullable: false),
                    UsuarioIdAccion = table.Column<int>(type: "int", nullable: false),
                    TipoOperacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CampoModificado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValorAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaOperacion = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCambiosLog", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroEmpleado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCambiosLog_UsuarioIdAccion",
                table: "UsuarioCambiosLog",
                column: "UsuarioIdAccion");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCambiosLog_UsuarioIdAfectado",
                table: "UsuarioCambiosLog",
                column: "UsuarioIdAfectado");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NumeroEmpleado",
                table: "Usuarios",
                column: "NumeroEmpleado",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_BuscarUsuarios;");

            migrationBuilder.DropTable(
                name: "UsuarioCambiosLog");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
