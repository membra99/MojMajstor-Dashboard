using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class DeclarationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeclarationId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Declarations",
                columns: table => new
                {
                    DeclarationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeclarationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAndTypeOfProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Distributor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsumerRights = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declarations", x => x.DeclarationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_DeclarationId",
                table: "Products",
                column: "DeclarationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Declarations_DeclarationId",
                table: "Products",
                column: "DeclarationId",
                principalTable: "Declarations",
                principalColumn: "DeclarationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Declarations_DeclarationId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Declarations");

            migrationBuilder.DropIndex(
                name: "IX_Products_DeclarationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeclarationId",
                table: "Products");
        }
    }
}
