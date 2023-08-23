using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class SiteContentAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteContentTypes",
                columns: table => new
                {
                    SiteContentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteContentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContentTypes", x => x.SiteContentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediaId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                    table.ForeignKey(
                        name: "FK_Tags_Medias_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Medias",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiteContents",
                columns: table => new
                {
                    SiteContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteContentTypeId = table.Column<int>(type: "int", nullable: true),
                    MediaId = table.Column<int>(type: "int", nullable: true),
                    SeoId = table.Column<int>(type: "int", nullable: true),
                    TagId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContents", x => x.SiteContentId);
                    table.ForeignKey(
                        name: "FK_SiteContents_Medias_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Medias",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteContents_Seos_SeoId",
                        column: x => x.SeoId,
                        principalTable: "Seos",
                        principalColumn: "SeoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteContents_SiteContentTypes_SiteContentTypeId",
                        column: x => x.SiteContentTypeId,
                        principalTable: "SiteContentTypes",
                        principalColumn: "SiteContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteContents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteContents_MediaId",
                table: "SiteContents",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteContents_SeoId",
                table: "SiteContents",
                column: "SeoId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteContents_SiteContentTypeId",
                table: "SiteContents",
                column: "SiteContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteContents_TagId",
                table: "SiteContents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_MediaId",
                table: "Tags",
                column: "MediaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteContents");

            migrationBuilder.DropTable(
                name: "SiteContentTypes");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
