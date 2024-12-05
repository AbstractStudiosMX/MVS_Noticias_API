using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class CustomSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignInMethod",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AutosyMasOrder",
                table: "CustomSettings",
                newName: "AutosMasOrder");

            migrationBuilder.AlterColumn<bool>(
                name: "isDefaultOrder",
                table: "CustomSettings",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AutosMasOrder",
                table: "CustomSettings",
                newName: "AutosyMasOrder");

            migrationBuilder.AddColumn<string>(
                name: "SignInMethod",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "isDefaultOrder",
                table: "CustomSettings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
