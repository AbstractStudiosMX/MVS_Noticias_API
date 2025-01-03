using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class newpodcastmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioPublicAdFreeUrl",
                table: "savedPodcasts");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "savedPodcasts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioPublicAdFreeUrl",
                table: "savedPodcasts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "DurationSeconds",
                table: "savedPodcasts",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
