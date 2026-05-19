using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBodyMeasurementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BodyFatPercentage",
                table: "BodyMeasurements",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<decimal>(
                name: "Waist",
                table: "BodyMeasurements",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Hips",
                table: "BodyMeasurements",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "BodyMeasurements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TargetWeight",
                table: "Users",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Waist", table: "BodyMeasurements");
            migrationBuilder.DropColumn(name: "Hips", table: "BodyMeasurements");
            migrationBuilder.DropColumn(name: "Notes", table: "BodyMeasurements");
            migrationBuilder.DropColumn(name: "TargetWeight", table: "Users");

            migrationBuilder.AlterColumn<decimal>(
                name: "BodyFatPercentage",
                table: "BodyMeasurements",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
