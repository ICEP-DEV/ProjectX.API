using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectX.Data.Migrations
{
    /// <inheritdoc />
    public partial class dondatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Event",
                table: "Donation",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "EventOptions",
                table: "Donation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventOptions",
                table: "Donation");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Donation",
                newName: "Event");
        }
    }
}
