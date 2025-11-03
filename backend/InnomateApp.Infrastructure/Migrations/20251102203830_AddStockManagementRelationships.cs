using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnomateApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockManagementRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Roles_RolesId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Roles_RolesId",
                table: "RoleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Users_UsersId",
                table: "RoleUser");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "RoleUser",
                newName: "UsersUserId");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "RoleUser",
                newName: "RolesRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser",
                newName: "IX_RoleUser_UsersUserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Roles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Permissions",
                newName: "PermissionId");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "PermissionRole",
                newName: "RolesRoleId");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "PermissionRole",
                newName: "PermissionsPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRole_RolesId",
                table: "PermissionRole",
                newName: "IX_PermissionRole_RolesRoleId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.SaleId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_Purchases_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockSummaries",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StockSummaryId = table.Column<int>(type: "int", nullable: false),
                    TotalIn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOut = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSummaries", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_StockSummaries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    StockTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.StockTransactionId);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseDetails",
                columns: table => new
                {
                    PurchaseDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDetails", x => x.PurchaseDetailId);
                    table.ForeignKey(
                        name: "FK_PurchaseDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDetails_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "PurchaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleDetails",
                columns: table => new
                {
                    SaleDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDetailId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDetails", x => x.SaleDetailId);
                    table.ForeignKey(
                        name: "FK_SaleDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleDetails_PurchaseDetails_PurchaseDetailId",
                        column: x => x.PurchaseDetailId,
                        principalTable: "PurchaseDetails",
                        principalColumn: "PurchaseDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleDetails_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "SaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetails_ProductId",
                table: "PurchaseDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetails_PurchaseId",
                table: "PurchaseDetails",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SupplierId",
                table: "Purchases",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_ProductId",
                table: "SaleDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_PurchaseDetailId",
                table: "SaleDetails",
                column: "PurchaseDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_SaleId",
                table: "SaleDetails",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId",
                table: "StockTransactions",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsPermissionId",
                table: "PermissionRole",
                column: "PermissionsPermissionId",
                principalTable: "Permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Roles_RolesRoleId",
                table: "PermissionRole",
                column: "RolesRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Roles_RolesRoleId",
                table: "RoleUser",
                column: "RolesRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Users_UsersUserId",
                table: "RoleUser",
                column: "UsersUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsPermissionId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Roles_RolesRoleId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Roles_RolesRoleId",
                table: "RoleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Users_UsersUserId",
                table: "RoleUser");

            migrationBuilder.DropTable(
                name: "SaleDetails");

            migrationBuilder.DropTable(
                name: "StockSummaries");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "PurchaseDetails");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UsersUserId",
                table: "RoleUser",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "RolesRoleId",
                table: "RoleUser",
                newName: "RolesId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleUser_UsersUserId",
                table: "RoleUser",
                newName: "IX_RoleUser_UsersId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "Permissions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RolesRoleId",
                table: "PermissionRole",
                newName: "RolesId");

            migrationBuilder.RenameColumn(
                name: "PermissionsPermissionId",
                table: "PermissionRole",
                newName: "PermissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRole_RolesRoleId",
                table: "PermissionRole",
                newName: "IX_PermissionRole_RolesId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsId",
                table: "PermissionRole",
                column: "PermissionsId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Roles_RolesId",
                table: "PermissionRole",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Roles_RolesId",
                table: "RoleUser",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Users_UsersId",
                table: "RoleUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
