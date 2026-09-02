using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnifyERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCodingChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodingChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    FacultyUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    InputFormat = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OutputFormat = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SampleInput = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SampleOutput = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodingChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodingChallenges_AspNetUsers_FacultyUserId",
                        column: x => x.FacultyUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodingChallenges_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CodingSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeId = table.Column<int>(type: "int", nullable: false),
                    StudentUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceCode = table.Column<string>(type: "nvarchar(max)", maxLength: 30000, nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodingSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodingSubmissions_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodingSubmissions_CodingChallenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "CodingChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodingChallenges_CourseId",
                table: "CodingChallenges",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CodingChallenges_FacultyUserId",
                table: "CodingChallenges",
                column: "FacultyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CodingSubmissions_ChallengeId_StudentUserId_SubmittedAtUtc",
                table: "CodingSubmissions",
                columns: new[] { "ChallengeId", "StudentUserId", "SubmittedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CodingSubmissions_StudentUserId",
                table: "CodingSubmissions",
                column: "StudentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodingSubmissions");

            migrationBuilder.DropTable(
                name: "CodingChallenges");
        }
    }
}
