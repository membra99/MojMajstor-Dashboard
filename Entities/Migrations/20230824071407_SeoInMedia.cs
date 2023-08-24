using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class SeoInMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltTitle",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltTitle",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Medias");
        }
    }
}
