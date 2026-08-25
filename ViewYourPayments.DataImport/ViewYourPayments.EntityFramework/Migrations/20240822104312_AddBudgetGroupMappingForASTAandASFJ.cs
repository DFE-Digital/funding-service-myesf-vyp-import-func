using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ViewYourPayments.EntityFramework.Migrations
{
    public partial class AddBudgetGroupMappingForASTAandASFJ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BudgetGroups",
                columns: new[] { "Id", "BudgetGroupDescription", "ContractCodePrefix", "CreatedOn" },
                values: new object[,]
                {
                    { 57, "Non-Procured Adult Skills Fund", "ASCA", new DateTime(2024, 08, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 58, "Non-Procured Free Courses for Job", "ASFC", new DateTime(2024, 08, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BudgetGroups",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "BudgetGroups",
                keyColumn: "Id",
                keyValue: 58);
        }
    }
}
