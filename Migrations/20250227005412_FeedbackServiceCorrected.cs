using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackServiceCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "Feedback",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTypeId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_ServiceTypeId",
                table: "Feedback",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_ServiceTypes_ServiceTypeId",
                table: "Feedback",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_ServiceTypes_ServiceTypeId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_ServiceTypeId",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "Feedback");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTypeId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
