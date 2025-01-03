using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class tablesname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_savedPodcasts_Users_UserId",
                table: "savedPodcasts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_savedPodcasts",
                table: "savedPodcasts");

            migrationBuilder.RenameTable(
                name: "savedPodcasts",
                newName: "SavedPodcasts");

            migrationBuilder.RenameIndex(
                name: "IX_savedPodcasts_UserId",
                table: "SavedPodcasts",
                newName: "IX_SavedPodcasts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedPodcasts",
                table: "SavedPodcasts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPodcasts_Users_UserId",
                table: "SavedPodcasts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedPodcasts_Users_UserId",
                table: "SavedPodcasts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedPodcasts",
                table: "SavedPodcasts");

            migrationBuilder.RenameTable(
                name: "SavedPodcasts",
                newName: "savedPodcasts");

            migrationBuilder.RenameIndex(
                name: "IX_SavedPodcasts_UserId",
                table: "savedPodcasts",
                newName: "IX_savedPodcasts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_savedPodcasts",
                table: "savedPodcasts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_savedPodcasts_Users_UserId",
                table: "savedPodcasts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
