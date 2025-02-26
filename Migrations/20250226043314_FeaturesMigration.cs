using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAPI.Migrations
{
    /// <inheritdoc />
    public partial class FeaturesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureServiceType_Feature_FeaturesId",
                table: "FeatureServiceType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feature",
                table: "Feature");

            migrationBuilder.RenameTable(
                name: "Feature",
                newName: "Features");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Features",
                table: "Features",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureServiceType_Features_FeaturesId",
                table: "FeatureServiceType",
                column: "FeaturesId",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureServiceType_Features_FeaturesId",
                table: "FeatureServiceType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Features",
                table: "Features");

            migrationBuilder.RenameTable(
                name: "Features",
                newName: "Feature");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feature",
                table: "Feature",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureServiceType_Feature_FeaturesId",
                table: "FeatureServiceType",
                column: "FeaturesId",
                principalTable: "Feature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
