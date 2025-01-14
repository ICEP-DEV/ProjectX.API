using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectX.Data.Migrations
{
    /// <inheritdoc />
    public partial class rsvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RSVPs_Alumnus_AlumnusId",
                table: "RSVPs");

            migrationBuilder.DropForeignKey(
                name: "FK_RSVPs_Event_EventId",
                table: "RSVPs");

            migrationBuilder.DropIndex(
                name: "IX_RSVPs_AlumnusId",
                table: "RSVPs");

            migrationBuilder.DropIndex(
                name: "IX_RSVPs_EventId",
                table: "RSVPs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_AlumnusId",
                table: "RSVPs",
                column: "AlumnusId");

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_EventId",
                table: "RSVPs",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSVPs_Alumnus_AlumnusId",
                table: "RSVPs",
                column: "AlumnusId",
                principalTable: "Alumnus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RSVPs_Event_EventId",
                table: "RSVPs",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
