using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnomateApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class stocksupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfitMargin",
                table: "Sales",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalProfit",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Sales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AddColumn<decimal>(
                name: "Profit",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SaleDetailBatches",
                columns: table => new
                {
                    SaleDetailBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleDetailId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDetailId = table.Column<int>(type: "int", nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDetailBatches", x => x.SaleDetailBatchId);
                    table.ForeignKey(
                        name: "FK_SaleDetailBatches_PurchaseDetails_PurchaseDetailId",
                        column: x => x.PurchaseDetailId,
                        principalTable: "PurchaseDetails",
                        principalColumn: "PurchaseDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleDetailBatches_SaleDetails_SaleDetailId",
                        column: x => x.SaleDetailId,
                        principalTable: "SaleDetails",
                        principalColumn: "SaleDetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetailBatches_PurchaseDetailId",
                table: "SaleDetailBatches",
                column: "PurchaseDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetailBatches_SaleDetailId",
                table: "SaleDetailBatches",
                column: "SaleDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleDetailBatches");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ProfitMargin",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalProfit",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "SaleDetails");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "Sales",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "SaleDetails",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "SaleDetails",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SaleDetails",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
