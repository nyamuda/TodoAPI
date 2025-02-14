using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class CancelledByUserOwned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelDetails_Users_CancelledByUserId",
                table: "CancelDetails");

            migrationBuilder.DropIndex(
                name: "IX_CancelDetails_CancelledByUserId",
                table: "CancelDetails");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "CancelDetails");

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUser_Name",
                table: "CancelDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUser_Role",
                table: "CancelDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledByUser_Name",
                table: "CancelDetails");

            migrationBuilder.DropColumn(
                name: "CancelledByUser_Role",
                table: "CancelDetails");

            migrationBuilder.AddColumn<int>(
                name: "CancelledByUserId",
                table: "CancelDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CancelDetails_CancelledByUserId",
                table: "CancelDetails",
                column: "CancelledByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelDetails_Users_CancelledByUserId",
                table: "CancelDetails",
                column: "CancelledByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
