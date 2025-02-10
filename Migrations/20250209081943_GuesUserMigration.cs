using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class GuesUserMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GuestPhone",
                table: "Bookings",
                newName: "GuestUser_Phone");

            migrationBuilder.RenameColumn(
                name: "GuestName",
                table: "Bookings",
                newName: "GuestUser_Name");

            migrationBuilder.RenameColumn(
                name: "GuestEmail",
                table: "Bookings",
                newName: "GuestUser_Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GuestUser_Phone",
                table: "Bookings",
                newName: "GuestPhone");

            migrationBuilder.RenameColumn(
                name: "GuestUser_Name",
                table: "Bookings",
                newName: "GuestName");

            migrationBuilder.RenameColumn(
                name: "GuestUser_Email",
                table: "Bookings",
                newName: "GuestEmail");
        }
    }
}
