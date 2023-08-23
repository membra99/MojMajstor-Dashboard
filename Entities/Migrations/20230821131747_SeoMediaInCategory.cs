using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class SeoMediaInCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeoId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_MediaId",
                table: "Categories",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SeoId",
                table: "Categories",
                column: "SeoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Seos_SeoId",
                table: "Categories",
                column: "SeoId",
                principalTable: "Seos",
                principalColumn: "SeoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Seos_SeoId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_MediaId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SeoId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SeoId",
                table: "Categories");
        }
    }
}
