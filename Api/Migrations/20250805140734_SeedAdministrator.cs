using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_administrators",
                table: "administrators");

            migrationBuilder.RenameTable(
                name: "administrators",
                newName: "Administrators");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Administrators",
                table: "Administrators",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Administrators",
                columns: new[] { "Id", "Email", "Password", "Profile" },
                values: new object[] { 1, "administrator@teste.com", "123456", "Adm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Administrators",
                table: "Administrators");

            migrationBuilder.DeleteData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameTable(
                name: "Administrators",
                newName: "administrators");

            migrationBuilder.AddPrimaryKey(
                name: "PK_administrators",
                table: "administrators",
                column: "Id");
        }
    }
}
