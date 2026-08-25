using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ViewYourPayments.EntityFramework.Migrations
{
    public partial class AddedDbTableStructureAndData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractCodePrefix = table.Column<string>(maxLength: 10, nullable: true),
                    BudgetGroupDescription = table.Column<string>(maxLength: 500, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataImportAudits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Severity = table.Column<int>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataImportAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataImportHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BatchId = table.Column<int>(nullable: false),
                    SkipTokenNumber = table.Column<int>(nullable: false),
                    RunFromDate = table.Column<DateTime>(nullable: false),
                    RunToDate = table.Column<DateTime>(nullable: false),
                    RequestedUrl = table.Column<string>(maxLength: 1000, nullable: true),
                    TotalRecords = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataImportHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentIdentifier = table.Column<int>(nullable: false),
                    Ukprn = table.Column<string>(maxLength: 50, nullable: true),
                    PaymentTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    VendorIdentifier = table.Column<string>(maxLength: 1000, nullable: true),
                    DataImportHistoryId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSummaries_DataImportHistories_DataImportHistoryId",
                        column: x => x.DataImportHistoryId,
                        principalTable: "DataImportHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentLines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentLineIdentifier = table.Column<long>(nullable: false),
                    Contract = table.Column<string>(maxLength: 200, nullable: true),
                    Establishment = table.Column<string>(maxLength: 200, nullable: true),
                    EstablishmentDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    PaymentLineDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    LineDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    PaymentSummaryId = table.Column<int>(nullable: false),
                    LineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "GetDate()"),
                    FundingType = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(nullable: false),
                    BudgetGroup = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentLines_PaymentSummaries_PaymentSummaryId",
                        column: x => x.PaymentSummaryId,
                        principalTable: "PaymentSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BudgetGroups",
                columns: new[] { "Id", "BudgetGroupDescription", "ContractCodePrefix", "CreatedOn" },
                values: new object[,]
                {
                    { 1, "16-18 Traineeships", "16TR", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, "National Careers Service", "NCSI", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, "National Careers Service", "NCSP", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, "National Careers Service", "NCSS", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, "National Careers Service", "NCSV", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, "Non-Procured Adult Education Budget", "AEC", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, "Non-Procured Adult Education Budget", "AECA", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, "Non-Procured Adult Education Budget", "AECL", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, "Non-Procured Adult Education Budget", "AECT", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, "Non-Procured Adult Education Budget", "AELS", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, "Non-Procured Adult Education Budget", "AEO", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, "Procured Adult Education Budget", "AEBL", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, "Procured Adult Education Budget", "AEBT", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, "Procured Adult Education Budget", "AEAL", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, "Procured Adult Education Budget", "AEBA", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 40, "Procured Adult Education Budget", "AEBC", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 41, "Procured Adult Education Budget", "AEBR", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 42, "Procured Adult Education Budget", "AETL", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 43, "Procured Non-Levy Apprenticeships", "ANAS", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 44, "Procured Non-Levy Apprenticeships", "ANLP", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, "National Careers Service", "NCSF", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 45, "Procured Non-Levy Apprenticeships", "YNAS", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, "National Careers Service", "NCSE", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, "National Careers Service", "NCSC", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "16-19 Funding", "16ED", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "16-19 Funding", "16LR", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "16-19 Funding", "NLG", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "16-19 Funding", "NMF", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "16-19 Funding", "SSF", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Advanced Learner Loans", "ALLB", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Advanced Learner Loans", "ALLC", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "Advanced Learner Loans", "ALLF", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "Advanced Learner Loans", "CLP", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "Apprenticeship Grant for Employers", "AGE", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, "Apprenticeship (Employer on App service) Levy", "LEVY", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, "Apprenticeship (Employer on App service) Non-Levy", "NLA", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, "Apprenticeships Carry-in", "16AP", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, "Apprenticeships Carry-in", "16NL", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, "Apprenticeships Carry-in", "AAPP", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, "Apprenticeships Carry-in", "ANL", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, "Apprenticeships Carry-in", "APPS", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, "European Social Fund", "ESF", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, "National Careers Service", "NCSA", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, "National Careers Service", "NCSD", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 46, "Procured Non-Levy Apprenticeships", "YNLP", new DateTime(2019, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetGroups_ContractCodePrefix",
                table: "BudgetGroups",
                column: "ContractCodePrefix",
                unique: true,
                filter: "[ContractCodePrefix] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DataImportHistories_BatchId",
                table: "DataImportHistories",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLines_PaymentLineIdentifier",
                table: "PaymentLines",
                column: "PaymentLineIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLines_PaymentSummaryId",
                table: "PaymentLines",
                column: "PaymentSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLines_Contract_Establishment_LineDescription_LineAmount",
                table: "PaymentLines",
                columns: new[] { "Contract", "Establishment", "LineDescription", "LineAmount" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSummaries_DataImportHistoryId",
                table: "PaymentSummaries",
                column: "DataImportHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSummaries_PaymentIdentifier",
                table: "PaymentSummaries",
                column: "PaymentIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSummaries_PaymentDate_Ukprn_PaymentTotal",
                table: "PaymentSummaries",
                columns: new[] { "PaymentDate", "Ukprn", "PaymentTotal" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetGroups");

            migrationBuilder.DropTable(
                name: "DataImportAudits");

            migrationBuilder.DropTable(
                name: "PaymentLines");

            migrationBuilder.DropTable(
                name: "PaymentSummaries");

            migrationBuilder.DropTable(
                name: "DataImportHistories");
        }
    }
}
