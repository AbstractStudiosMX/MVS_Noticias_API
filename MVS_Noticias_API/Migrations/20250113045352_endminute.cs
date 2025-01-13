using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class endminute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Minute",
                table: "BroadcastInfo",
                newName: "StartMinute");

            migrationBuilder.RenameColumn(
                name: "Hour",
                table: "BroadcastInfo",
                newName: "StartHour");

            migrationBuilder.AddColumn<int>(
                name: "EndMinute",
                table: "BroadcastInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndMinute",
                table: "BroadcastInfo");

            migrationBuilder.RenameColumn(
                name: "StartMinute",
                table: "BroadcastInfo",
                newName: "Minute");

            migrationBuilder.RenameColumn(
                name: "StartHour",
                table: "BroadcastInfo",
                newName: "Hour");
        }
    }
}
