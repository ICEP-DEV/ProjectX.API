using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectX.Data.Migrations
{
    /// <inheritdoc />
    public partial class anotherDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_admin",
                table: "admin");

            migrationBuilder.RenameTable(
                name: "admin",
                newName: "Admin");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admin",
                table: "Admin",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Admin",
                table: "Admin");

            migrationBuilder.RenameTable(
                name: "Admin",
                newName: "admin");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admin",
                table: "admin",
                column: "Id");
        }
    }
}
