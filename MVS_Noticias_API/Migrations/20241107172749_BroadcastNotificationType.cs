using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class BroadcastNotificationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "BroadcastInfo");

            migrationBuilder.AddColumn<int>(
                name: "Hour",
                table: "BroadcastInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "BroadcastInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weekday",
                table: "BroadcastInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hour",
                table: "BroadcastInfo");

            migrationBuilder.DropColumn(
                name: "Minute",
                table: "BroadcastInfo");

            migrationBuilder.DropColumn(
                name: "Weekday",
                table: "BroadcastInfo");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "BroadcastInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
