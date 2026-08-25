using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViewYourPayments.EntityFramework.Migrations
{
    public partial class RemoveUniqueInPaymentLines_PaymentLineIdentifierIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentLines_PaymentLineIdentifier",
                table: "PaymentLines");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLines_PaymentLineIdentifier",
                table: "PaymentLines",
                column: "PaymentLineIdentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentLines_PaymentLineIdentifier",
                table: "PaymentLines");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLines_PaymentLineIdentifier",
                table: "PaymentLines",
                column: "PaymentLineIdentifier",
                unique: true);
        }
    }
}
