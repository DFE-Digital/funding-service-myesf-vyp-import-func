using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ViewYourPayments.EntityFramework.Migrations
{
    public partial class AddStagingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentSummariesStaging",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentIdentifier = table.Column<int>(type: "int", nullable: false),
                    Ukprn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VendorIdentifier = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataImportHistoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSummariesStaging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSummariesStaging_DataImportHistories_DataImportHistoryId",
                        column: x => x.DataImportHistoryId,
                        principalTable: "DataImportHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentLinesStaging",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentLineIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Contract = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Establishment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstablishmentDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentLineDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentLineGroupDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentSummaryId = table.Column<int>(type: "int", nullable: false),
                    LineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FundingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BudgetGroup = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLinesStaging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentLinesStaging_PaymentSummariesStaging_PaymentSummaryId",
                        column: x => x.PaymentSummaryId,
                        principalTable: "PaymentSummariesStaging",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinesStaging_PaymentSummaryId",
                table: "PaymentLinesStaging",
                column: "PaymentSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSummariesStaging_DataImportHistoryId",
                table: "PaymentSummariesStaging",
                column: "DataImportHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentLinesStaging");

            migrationBuilder.DropTable(
                name: "PaymentSummariesStaging");
        }
    }
}
