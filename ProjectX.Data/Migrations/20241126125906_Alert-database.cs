using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectX.Data.Migrations
{
    /// <inheritdoc />
    public partial class Alertdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AlumnusProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "AlumnusProfile");
        }
    }
}
