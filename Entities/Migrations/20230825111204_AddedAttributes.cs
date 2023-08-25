using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributes_Categories_CategoriesId",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ProductAttributes");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "ProductAttributes",
                newName: "AttributesId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributes_CategoriesId",
                table: "ProductAttributes",
                newName: "IX_ProductAttributes_AttributesId");

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    AttributesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.AttributesId);
                    table.ForeignKey(
                        name: "FK_Attributes_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_CategoriesId",
                table: "Attributes",
                column: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributes_Attributes_AttributesId",
                table: "ProductAttributes",
                column: "AttributesId",
                principalTable: "Attributes",
                principalColumn: "AttributesId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributes_Attributes_AttributesId",
                table: "ProductAttributes");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.RenameColumn(
                name: "AttributesId",
                table: "ProductAttributes",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributes_AttributesId",
                table: "ProductAttributes",
                newName: "IX_ProductAttributes_CategoriesId");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributes_Categories_CategoriesId",
                table: "ProductAttributes",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
