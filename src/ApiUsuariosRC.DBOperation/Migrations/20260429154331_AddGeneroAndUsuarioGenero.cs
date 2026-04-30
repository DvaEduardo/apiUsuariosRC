using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiUsuariosRC.DBOperation.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneroAndUsuarioGenero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genero",
                columns: table => new
                {
                    GeneroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genero = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.GeneroId);
                });

            migrationBuilder.Sql(
                """
                SET IDENTITY_INSERT dbo.Genero ON;

                INSERT INTO dbo.Genero (GeneroId, Genero, FechaCreacion)
                VALUES
                    (1, N'Masculino', CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Central Standard Time (Mexico)' AS datetime2(0))),
                    (2, N'Femenino', CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Central Standard Time (Mexico)' AS datetime2(0)));

                SET IDENTITY_INSERT dbo.Genero OFF;
                """);

            migrationBuilder.AddColumn<int>(
                name: "GeneroId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                """
                UPDATE dbo.Usuarios
                   SET NumeroEmpleado = REPLACE(NumeroEmpleado, 'EMX-', 'RC-M-')
                 WHERE NumeroEmpleado LIKE 'EMX-%'
                   AND NumeroEmpleado NOT LIKE 'EMX-[A-Z]-%';

                UPDATE dbo.Usuarios
                   SET NumeroEmpleado = REPLACE(NumeroEmpleado, 'EMX-', 'RC-')
                 WHERE NumeroEmpleado LIKE 'EMX-[A-Z]-%';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GeneroId",
                table: "Usuarios",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Genero_Genero",
                table: "Genero",
                column: "Genero",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Genero_GeneroId",
                table: "Usuarios",
                column: "GeneroId",
                principalTable: "Genero",
                principalColumn: "GeneroId",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Genero_GeneroId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_GeneroId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GeneroId",
                table: "Usuarios");
        }
    }
}
