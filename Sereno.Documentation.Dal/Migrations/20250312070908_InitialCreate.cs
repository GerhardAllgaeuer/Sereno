using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sereno.Documentation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "docDocument",
                columns: table => new
                {
                    vId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    vLibraryPath = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    vDocumentKey = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    vTitle = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    vContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vHtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vAuthor = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    dNextCheck = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vCreateUser = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vModifyUser = table.Column<string>(type: "nvarchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_docDocument", x => x.vId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "docDocument");
        }
    }
}
