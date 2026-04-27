using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplySync.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorApplicantAndUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "RoleType" },
                values: new object[] { 7, "VendorApplicant" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 7);
        }
    }
}
