using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class ImageIdNullMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypes_Images_ImageId",
                table: "ServiceTypes");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypes_Images_ImageId",
                table: "ServiceTypes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypes_Images_ImageId",
                table: "ServiceTypes");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypes_Images_ImageId",
                table: "ServiceTypes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }
    }
}
