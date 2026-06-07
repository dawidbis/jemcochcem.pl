using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMicroNutrients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCalcium",
                table: "MealLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFiber",
                table: "MealLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIron",
                table: "MealLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSaturatedFat",
                table: "MealLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSodium",
                table: "MealLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSugars",
                table: "MealLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CalciumPer100g",
                table: "FoodProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FiberPer100g",
                table: "FoodProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IronPer100g",
                table: "FoodProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaturatedFatPer100g",
                table: "FoodProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SodiumPer100g",
                table: "FoodProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SugarsPer100g",
                table: "FoodProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCalcium",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "TotalFiber",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "TotalIron",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "TotalSaturatedFat",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "TotalSodium",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "TotalSugars",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "CalciumPer100g",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "FiberPer100g",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "IronPer100g",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "SaturatedFatPer100g",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "SodiumPer100g",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "SugarsPer100g",
                table: "FoodProducts");
        }
    }
}
