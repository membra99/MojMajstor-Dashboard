using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class addedLanguagesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "SiteContents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Seos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Declarations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LanguageID",
                table: "Tags",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteContents_LanguageID",
                table: "SiteContents",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_Seos_LanguageID",
                table: "Seos",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LanguageID",
                table: "Products",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_Declarations_LanguageID",
                table: "Declarations",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_LanguageID",
                table: "Categories",
                column: "LanguageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Languages_LanguageID",
                table: "Categories",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Declarations_Languages_LanguageID",
                table: "Declarations",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Languages_LanguageID",
                table: "Products",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Seos_Languages_LanguageID",
                table: "Seos",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteContents_Languages_LanguageID",
                table: "SiteContents",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Languages_LanguageID",
                table: "Tags",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Languages_LanguageID",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Declarations_Languages_LanguageID",
                table: "Declarations");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Languages_LanguageID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Seos_Languages_LanguageID",
                table: "Seos");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteContents_Languages_LanguageID",
                table: "SiteContents");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Languages_LanguageID",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Tags_LanguageID",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_SiteContents_LanguageID",
                table: "SiteContents");

            migrationBuilder.DropIndex(
                name: "IX_Seos_LanguageID",
                table: "Seos");

            migrationBuilder.DropIndex(
                name: "IX_Products_LanguageID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Declarations_LanguageID",
                table: "Declarations");

            migrationBuilder.DropIndex(
                name: "IX_Categories_LanguageID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "SiteContents");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Seos");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Declarations");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Categories");
        }
    }
}
