using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnomateApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                });

            // 1. Seed default tenant (1)
            migrationBuilder.Sql("SET IDENTITY_INSERT Tenants ON;");
            migrationBuilder.Sql("INSERT INTO Tenants (TenantId, Name, Code, IsActive, CreatedAt) VALUES (1, 'Default Tenant', 'DEFAULT', 1, GETUTCDATE());");
            migrationBuilder.Sql("SET IDENTITY_INSERT Tenants OFF;");

            // 2. Update existing records from 0 to 1 (since default value in previous migration was 0)
            migrationBuilder.Sql("UPDATE Users SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Categories SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Products SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Suppliers SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Purchases SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE PurchaseDetails SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Sales SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE SaleDetails SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE ReturnDetails SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Returns SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE StockSummaries SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE StockTransactions SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Payments SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE Customers SET TenantId = 1 WHERE TenantId = 0;");
            migrationBuilder.Sql("UPDATE SaleDetailBatches SET TenantId = 1 WHERE TenantId = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");
        }
    }
}
