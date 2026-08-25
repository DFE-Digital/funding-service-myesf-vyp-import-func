using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViewYourPayments.EntityFramework.Migrations
{
    public partial class RemoveUniqueInPaymentSummary_PaymentIdentifierIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentSummaries_PaymentIdentifier",
                table: "PaymentSummaries");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSummaries_PaymentIdentifier",
                table: "PaymentSummaries",
                column: "PaymentIdentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentSummaries_PaymentIdentifier",
                table: "PaymentSummaries");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSummaries_PaymentIdentifier",
                table: "PaymentSummaries",
                column: "PaymentIdentifier",
                unique: true);
        }
    }
}
