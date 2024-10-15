using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilitySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FontSize = table.Column<int>(type: "int", nullable: false),
                    ApareanceMode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilitySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessibilitySettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GuardadosOrder = table.Column<int>(type: "int", nullable: false),
                    MasLeidasOrder = table.Column<int>(type: "int", nullable: false),
                    TendenciasOrder = table.Column<int>(type: "int", nullable: false),
                    EntrevistasOrder = table.Column<int>(type: "int", nullable: false),
                    MVSDeportesOrder = table.Column<int>(type: "int", nullable: false),
                    NacionalOrder = table.Column<int>(type: "int", nullable: false),
                    VideosOrder = table.Column<int>(type: "int", nullable: false),
                    CDMXOrder = table.Column<int>(type: "int", nullable: false),
                    EntretenimientoOrder = table.Column<int>(type: "int", nullable: false),
                    OpinionOrder = table.Column<int>(type: "int", nullable: false),
                    EconomiaOrder = table.Column<int>(type: "int", nullable: false),
                    EstadosOrder = table.Column<int>(type: "int", nullable: false),
                    MundoOrder = table.Column<int>(type: "int", nullable: false),
                    MascotasOrder = table.Column<int>(type: "int", nullable: false),
                    SaludBienestarOrder = table.Column<int>(type: "int", nullable: false),
                    PoliciacaOrder = table.Column<int>(type: "int", nullable: false),
                    Programacion = table.Column<int>(type: "int", nullable: false),
                    CienciaTecnologiaOrder = table.Column<int>(type: "int", nullable: false),
                    ViralOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Tendencias = table.Column<bool>(type: "bit", nullable: false),
                    Entrevistas = table.Column<bool>(type: "bit", nullable: false),
                    MVSDeportes = table.Column<bool>(type: "bit", nullable: false),
                    Nacional = table.Column<bool>(type: "bit", nullable: false),
                    Videos = table.Column<bool>(type: "bit", nullable: false),
                    CDMX = table.Column<bool>(type: "bit", nullable: false),
                    Entretenimiento = table.Column<bool>(type: "bit", nullable: false),
                    Opinion = table.Column<bool>(type: "bit", nullable: false),
                    Economia = table.Column<bool>(type: "bit", nullable: false),
                    Estados = table.Column<bool>(type: "bit", nullable: false),
                    Mundo = table.Column<bool>(type: "bit", nullable: false),
                    Mascotas = table.Column<bool>(type: "bit", nullable: false),
                    SaludBienestar = table.Column<bool>(type: "bit", nullable: false),
                    Policiaca = table.Column<bool>(type: "bit", nullable: false),
                    Programacion = table.Column<bool>(type: "bit", nullable: false),
                    CienciaTecnologia = table.Column<bool>(type: "bit", nullable: false),
                    Viral = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SectionsAndIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedNews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessibilitySettings_UserId",
                table: "AccessibilitySettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomSettings_UserId",
                table: "CustomSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsSettings_UserId",
                table: "NotificationsSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedNews_UserId",
                table: "SavedNews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessibilitySettings");

            migrationBuilder.DropTable(
                name: "CustomSettings");

            migrationBuilder.DropTable(
                name: "NotificationsSettings");

            migrationBuilder.DropTable(
                name: "SavedNews");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
