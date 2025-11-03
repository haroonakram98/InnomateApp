using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnomateApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuthAndStockManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "DefaultUnitPrice",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "StockTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "StockTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageCost",
                table: "StockSummaries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "StockSummaries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "SaleDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "PurchaseDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "PurchaseDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSalePrice",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "AverageCost",
                table: "StockSummaries");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "StockSummaries");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "DefaultSalePrice",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "StockTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "PurchaseDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultUnitPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
