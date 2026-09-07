using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueOpenContractPerCustomerAndPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contracts_CustomerId",
                table: "contracts");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_CustomerId_PlanId_Open",
                table: "contracts",
                columns: new[] { "CustomerId", "PlanId" },
                unique: true,
                filter: "[Status] <> 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contracts_CustomerId_PlanId_Open",
                table: "contracts");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_CustomerId",
                table: "contracts",
                column: "CustomerId");
        }
    }
}
