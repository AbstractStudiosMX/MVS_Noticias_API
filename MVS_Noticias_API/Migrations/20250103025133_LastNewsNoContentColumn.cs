using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class LastNewsNoContentColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "LastNews");

            migrationBuilder.DropColumn(
                name: "HiddenTags",
                table: "LastNews");

            migrationBuilder.DropColumn(
                name: "NewsQuantity",
                table: "LastNews");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "LastNews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "LastNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HiddenTags",
                table: "LastNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NewsQuantity",
                table: "LastNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "LastNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
