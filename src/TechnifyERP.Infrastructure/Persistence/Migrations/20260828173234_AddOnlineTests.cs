using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnifyERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOnlineTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    FacultyUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalMarks = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTests_AspNetUsers_FacultyUserId",
                        column: x => x.FacultyUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTests_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CorrectOption = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Marks = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestQuestions_CourseTests_TestId",
                        column: x => x.TestId,
                        principalTable: "CourseTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    StudentUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_CourseTests_TestId",
                        column: x => x.TestId,
                        principalTable: "CourseTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOption = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    MarksAwarded = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAnswers_TestQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "TestQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAnswers_TestResults_TestResultId",
                        column: x => x.TestResultId,
                        principalTable: "TestResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTests_CourseId",
                table: "CourseTests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTests_FacultyUserId",
                table: "CourseTests",
                column: "FacultyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAnswers_QuestionId",
                table: "TestAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAnswers_TestResultId_QuestionId",
                table: "TestAnswers",
                columns: new[] { "TestResultId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_TestId",
                table: "TestQuestions",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_StudentUserId",
                table: "TestResults",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestId_StudentUserId",
                table: "TestResults",
                columns: new[] { "TestId", "StudentUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestAnswers");

            migrationBuilder.DropTable(
                name: "TestQuestions");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "CourseTests");
        }
    }
}
