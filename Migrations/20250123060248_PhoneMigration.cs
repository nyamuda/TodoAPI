using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class PhoneMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Items",
                newName: "VehicleType");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Items",
                newName: "ScheduledAt");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Items",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestPhone",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "GuestPhone",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "VehicleType",
                table: "Items",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Items",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ScheduledAt",
                table: "Items",
                newName: "DueDate");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
