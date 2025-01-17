using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class missingVariablesOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guardados",
                table: "NotificationsSettings");

            migrationBuilder.AddColumn<int>(
                name: "GuardadosOrder",
                table: "CustomSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MultimediaOrder",
                table: "CustomSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UltimaHoraOrder",
                table: "CustomSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuardadosOrder",
                table: "CustomSettings");

            migrationBuilder.DropColumn(
                name: "MultimediaOrder",
                table: "CustomSettings");

            migrationBuilder.DropColumn(
                name: "UltimaHoraOrder",
                table: "CustomSettings");

            migrationBuilder.AddColumn<bool>(
                name: "Guardados",
                table: "NotificationsSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
