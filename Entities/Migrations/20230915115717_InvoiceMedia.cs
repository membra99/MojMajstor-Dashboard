using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaId",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_MediaId",
                table: "Invoices",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Medias_MediaId",
                table: "Invoices",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Medias_MediaId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_MediaId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "Invoices");
        }
    }
}
