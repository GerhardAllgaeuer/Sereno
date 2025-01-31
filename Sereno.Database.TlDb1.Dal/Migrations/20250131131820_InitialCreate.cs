using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sereno.Database.TlDb1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tstSimple",
                columns: table => new
                {
                    vId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    vTitle = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    vDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vCreateUser = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vModifyUser = table.Column<string>(type: "nvarchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tstSimple", x => x.vId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tstSimple");
        }
    }
}
