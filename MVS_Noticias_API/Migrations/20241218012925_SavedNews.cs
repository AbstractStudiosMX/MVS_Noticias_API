using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    /// <inheritdoc />
    public partial class SavedNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShareLink",
                table: "SavedNews",
                newName: "VideoUrl");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "SavedNews",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Autor",
                table: "SavedNews",
                newName: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HiddenTags",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdAuthor",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCreator",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdNews",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdSection",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdSubSection",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSound",
                table: "SavedNews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVideo",
                table: "SavedNews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewsQuantity",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "SavedNews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoCredits",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoDescription",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoMobile",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoundUrl",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubSection",
                table: "SavedNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "HiddenTags",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IdAuthor",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IdCreator",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IdNews",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IdSection",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IdSubSection",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IsSound",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "IsVideo",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "NewsQuantity",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "PhotoCredits",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "PhotoDescription",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "PhotoMobile",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "SoundUrl",
                table: "SavedNews");

            migrationBuilder.DropColumn(
                name: "SubSection",
                table: "SavedNews");

            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "SavedNews",
                newName: "ShareLink");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "SavedNews",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "SavedNews",
                newName: "Autor");
        }
    }
}
