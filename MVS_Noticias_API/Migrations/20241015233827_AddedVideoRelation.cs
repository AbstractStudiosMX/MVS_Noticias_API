using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class AddedVideoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SectionsAndIds",
                table: "SavedNews",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShareLink",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SavedVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ViewsNumber = table.Column<int>(type: "int", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedVideos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedVideos_UserId",
                table: "SavedVideos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedVideos");

            migrationBuilder.DropColumn(
                name: "Autor",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "ShareLink",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SavedNews");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "SavedNews",
                newName: "SectionsAndIds");
        }
    }
}
