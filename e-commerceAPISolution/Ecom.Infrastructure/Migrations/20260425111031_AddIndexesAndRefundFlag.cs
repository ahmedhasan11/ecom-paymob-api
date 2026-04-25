using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndRefundFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_ExpiresAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_Status",
                table: "InventoryReservations");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresRefund",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId_Status",
                table: "Payments",
                columns: new[] { "OrderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status_ExpiresAt_CreatedAt",
                table: "Payments",
                columns: new[] { "Status", "ExpiresAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_Status_ExpiresAt",
                table: "InventoryReservations",
                columns: new[] { "Status", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId_Status",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status_ExpiresAt_CreatedAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_Status_ExpiresAt",
                table: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "RequiresRefund",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ExpiresAt",
                table: "Payments",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_Status",
                table: "InventoryReservations",
                column: "Status");
        }
    }
}
