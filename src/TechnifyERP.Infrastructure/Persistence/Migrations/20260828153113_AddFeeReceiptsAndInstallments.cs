using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnifyERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeReceiptsAndInstallments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminRemarks",
                table: "FeeRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReceiptPath",
                table: "FeeRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubmittedAmount",
                table: "FeeRecords",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "FeeRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstallmentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeRecordId = table.Column<int>(type: "int", nullable: false),
                    StudentUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstallmentRequests_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstallmentRequests_FeeRecords_FeeRecordId",
                        column: x => x.FeeRecordId,
                        principalTable: "FeeRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentRequests_FeeRecordId_StudentUserId_Status",
                table: "InstallmentRequests",
                columns: new[] { "FeeRecordId", "StudentUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentRequests_StudentUserId",
                table: "InstallmentRequests",
                column: "StudentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstallmentRequests");

            migrationBuilder.DropColumn(
                name: "AdminRemarks",
                table: "FeeRecords");

            migrationBuilder.DropColumn(
                name: "PaymentReceiptPath",
                table: "FeeRecords");

            migrationBuilder.DropColumn(
                name: "SubmittedAmount",
                table: "FeeRecords");

            migrationBuilder.DropColumn(
                name: "SubmittedAtUtc",
                table: "FeeRecords");
        }
    }
}
