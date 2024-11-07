using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class BroadcastList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BroadcastInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProgrammingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BroadcastInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BroadcastInfo_Programs_ProgrammingId",
                        column: x => x.ProgrammingId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BroadcastInfo_ProgrammingId",
                table: "BroadcastInfo",
                column: "ProgrammingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BroadcastInfo");
        }
    }
}
