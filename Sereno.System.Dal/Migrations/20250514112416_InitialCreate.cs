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
                name: "syrGrp",
                columns: table => new
                {
                    vId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    vDescription = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    dCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vCreateUser = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vModifyUser = table.Column<string>(type: "nvarchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_syrGrp", x => x.vId);
                });

            migrationBuilder.CreateTable(
                name: "syrUsr",
                columns: table => new
                {
                    vId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    vEmail = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    vHash = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    dCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vCreateUser = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vModifyUser = table.Column<string>(type: "nvarchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_syrUsr", x => x.vId);
                });

            migrationBuilder.CreateTable(
                name: "syrUsrGrp",
                columns: table => new
                {
                    vId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    vUserId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    vRoleId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    dCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vCreateUser = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vModifyUser = table.Column<string>(type: "nvarchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_syrUsrGrp", x => x.vId);
                    table.ForeignKey(
                        name: "FK_syrUsrGrp_syrGrp_vRoleId",
                        column: x => x.vRoleId,
                        principalTable: "syrGrp",
                        principalColumn: "vId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_syrUsrGrp_syrUsr_vUserId",
                        column: x => x.vUserId,
                        principalTable: "syrUsr",
                        principalColumn: "vId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_syrUsrGrp_vRoleId",
                table: "syrUsrGrp",
                column: "vRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_syrUsrGrp_vUserId",
                table: "syrUsrGrp",
                column: "vUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "syrUsrGrp");

            migrationBuilder.DropTable(
                name: "syrGrp");

            migrationBuilder.DropTable(
                name: "syrUsr");
        }
    }
}
