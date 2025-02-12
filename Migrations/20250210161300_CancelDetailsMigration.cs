using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class CancelDetailsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CancelReason",
                table: "Bookings",
                newName: "CancelDetails_CancelReason");

            migrationBuilder.AddColumn<int>(
                name: "CancelDetails_CancelledById",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CancelDetails_CancelledById",
                table: "Bookings",
                column: "CancelDetails_CancelledById");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_CancelDetails_CancelledById",
                table: "Bookings",
                column: "CancelDetails_CancelledById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_CancelDetails_CancelledById",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CancelDetails_CancelledById",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CancelDetails_CancelledById",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "CancelDetails_CancelReason",
                table: "Bookings",
                newName: "CancelReason");
        }
    }
}
