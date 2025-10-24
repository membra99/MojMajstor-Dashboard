using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations.MojMajstor
{
    /// <inheritdoc />
    public partial class UpdateMiedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Media");
        }
    }
}
