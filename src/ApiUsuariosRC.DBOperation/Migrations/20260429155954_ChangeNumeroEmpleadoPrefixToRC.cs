using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiUsuariosRC.DBOperation.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNumeroEmpleadoPrefixToRC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE dbo.Usuarios
                   SET NumeroEmpleado = REPLACE(NumeroEmpleado, 'EMX-', 'RC-')
                 WHERE NumeroEmpleado LIKE 'EMX-%';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE dbo.Usuarios
                   SET NumeroEmpleado = REPLACE(NumeroEmpleado, 'RC-', 'EMX-')
                 WHERE NumeroEmpleado LIKE 'RC-%';
                """);
        }
    }
}
