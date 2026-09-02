using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnifyERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentDoubts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentDoubts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    StudentUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Question = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AskedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AnsweredByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AnsweredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDoubts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDoubts_AspNetUsers_AnsweredByUserId",
                        column: x => x.AnsweredByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentDoubts_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentDoubts_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDoubts_AnsweredByUserId",
                table: "StudentDoubts",
                column: "AnsweredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDoubts_CourseId",
                table: "StudentDoubts",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDoubts_StudentUserId",
                table: "StudentDoubts",
                column: "StudentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDoubts");
        }
    }
}
