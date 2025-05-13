using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class InsertedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1cb7dacf-875a-4382-a43e-476c66668cca", null, "Visitor", "VISITOR" },
                    { "32c4e1fb-0b50-4352-9f5c-0825ddba8603", null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1cb7dacf-875a-4382-a43e-476c66668cca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32c4e1fb-0b50-4352-9f5c-0825ddba8603");
        }
    }
}
