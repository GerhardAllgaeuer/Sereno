using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sereno.Identity.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "syrUsr",
                columns: table => new
                {
                    vId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    vUserName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    vHash = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    vCreate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vCreateUser = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vModifyUser = table.Column<string>(type: "nvarchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_syrUsr", x => x.vId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "syrUsr");
        }
    }
}
