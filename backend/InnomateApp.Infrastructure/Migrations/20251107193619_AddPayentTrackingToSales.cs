using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnomateApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayentTrackingToSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BalanceAmount",
                table: "Sales",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFullyPaid",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Sales",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsFullyPaid",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Sales");
        }
    }
}
