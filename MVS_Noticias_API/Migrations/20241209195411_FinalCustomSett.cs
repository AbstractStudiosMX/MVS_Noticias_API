using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class FinalCustomSett : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutosMasOrder",
                table: "CustomSettings");

            migrationBuilder.DropColumn(
                name: "GuardadosOrder",
                table: "CustomSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutosMasOrder",
                table: "CustomSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GuardadosOrder",
                table: "CustomSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
