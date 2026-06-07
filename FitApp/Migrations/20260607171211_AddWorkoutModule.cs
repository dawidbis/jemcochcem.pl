using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitApp.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MuscleGroup = table.Column<string>(type: "text", nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SetEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetEntries_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetEntries_WorkoutSessions_WorkoutSessionId",
                        column: x => x.WorkoutSessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "IsCustom", "MuscleGroup", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("0755d1d9-54d5-48bb-82ac-8ad58029ba4c"), false, "Plecy", "Podciąganie na drążku", null },
                    { new Guid("0b529b22-6546-4522-8c7c-dbda75485c28"), false, "Barki", "Wyciskanie żołnierskie (OHP)", null },
                    { new Guid("36cf2c4b-31ae-4ca8-8578-fa485486b3ec"), false, "Nogi", "Prostowanie nóg na maszynie", null },
                    { new Guid("3eb0f25b-2f7b-457d-a2c8-3e9824b75560"), false, "Cardio", "Bieg / cardio", null },
                    { new Guid("52ed863f-f00c-4de8-8a27-035cf89faeea"), false, "Ramiona", "Uginanie ramion ze sztangą", null },
                    { new Guid("5d808eac-6a21-4a6a-b85c-3a97d0d38faf"), false, "Barki", "Wznosy bokiem", null },
                    { new Guid("75941a11-48c6-4853-8b94-b8b078cfdf9f"), false, "Plecy", "Martwy ciąg", null },
                    { new Guid("7eee9d25-94a3-4c08-b47d-136aca842828"), false, "Brzuch", "Unoszenie nóg", null },
                    { new Guid("900179c0-e035-41c8-8cff-41047d53f728"), false, "Nogi", "Wykroki z hantlami", null },
                    { new Guid("9753b98f-b41a-4411-bb8b-87af642b92e8"), false, "Ramiona", "Prostowanie ramion na wyciągu", null },
                    { new Guid("9e74f916-fb5a-49f6-b02d-039f2d874208"), false, "Plecy", "Wiosłowanie sztangą", null },
                    { new Guid("9eb50c5b-8614-4b2c-bc2f-4bcab75e9989"), false, "Klatka", "Wyciskanie sztangi leżąc", null },
                    { new Guid("a21bbae5-9d73-48d5-8b72-98a555183993"), false, "Nogi", "Przysiad ze sztangą", null },
                    { new Guid("b1150c95-cda4-493d-b637-48f51de164df"), false, "Klatka", "Rozpiętki na bramie", null },
                    { new Guid("bb543db9-e090-4d10-ba5c-f09c5d404b8c"), false, "Brzuch", "Plank", null },
                    { new Guid("d1200c1c-082a-45f2-b1c3-d16db2541031"), false, "Klatka", "Wyciskanie hantli na skosie", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetEntries_ExerciseId",
                table: "SetEntries",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_SetEntries_WorkoutSessionId",
                table: "SetEntries",
                column: "WorkoutSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SetEntries");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");
        }
    }
}
