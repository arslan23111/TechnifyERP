using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnifyERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyMaterialsAndLectures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordedLectures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    FacultyUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LectureNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordedLectures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordedLectures_AspNetUsers_FacultyUserId",
                        column: x => x.FacultyUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordedLectures_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    FacultyUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyMaterials_AspNetUsers_FacultyUserId",
                        column: x => x.FacultyUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyMaterials_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordedLectures_CourseId_LectureNumber",
                table: "RecordedLectures",
                columns: new[] { "CourseId", "LectureNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecordedLectures_FacultyUserId",
                table: "RecordedLectures",
                column: "FacultyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_CourseId",
                table: "StudyMaterials",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_FacultyUserId",
                table: "StudyMaterials",
                column: "FacultyUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordedLectures");

            migrationBuilder.DropTable(
                name: "StudyMaterials");
        }
    }
}
