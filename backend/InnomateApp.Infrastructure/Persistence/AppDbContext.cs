using InnomateApp.Domain.Entities;
using InnomateApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using InnomateApp.Application.Interfaces;

namespace InnomateApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantProvider? _tenantProvider;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider? tenantProvider = null) 
            : base(options) 
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
        public DbSet<StockSummary> StockSummaries => Set<StockSummary>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<Tax> Taxes => Set<Tax>();
        public DbSet<Return> Returns => Set<Return>();
        public DbSet<ReturnDetail> ReturnDetails => Set<ReturnDetail>();
        public DbSet<SaleDetailBatch> SaleDetailBatches => Set<SaleDetailBatch>();

        public int TenantIdFilter => _tenantProvider?.GetTenantId() ?? 0;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Many-to-Many: User <-> Role
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            // 🔹 Many-to-Many: Role <-> Permission
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles);

            // 🔹 User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // =====================================================
            // 🧩 Stock & Sales Management Relationships
            // =====================================================

            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);
            modelBuilder.Entity<Purchase>().HasKey(p => p.PurchaseId);
            modelBuilder.Entity<PurchaseDetail>().HasKey(pd => pd.PurchaseDetailId);
            modelBuilder.Entity<Sale>().HasKey(s => s.SaleId);
            modelBuilder.Entity<SaleDetail>().HasKey(sd => sd.SaleDetailId);
            modelBuilder.Entity<StockTransaction>().HasKey(st => st.StockTransactionId);
            modelBuilder.Entity<StockSummary>().HasKey(ss => ss.StockSummaryId);

            // 🔹 Category → Product (1:N)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Supplier → Purchase (1:N)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Purchase → PurchaseDetail (1:N)
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(d => d.Purchase)
                .WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Product → PurchaseDetail (1:N)
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(d => d.Product)
                .WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Sale → SaleDetail (1:N)
            modelBuilder.Entity<SaleDetail>()
                .HasOne(d => d.Sale)
                .WithMany(s => s.SaleDetails)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Product → SaleDetail (1:N)
            modelBuilder.Entity<SaleDetail>()
                .HasOne(d => d.Product)
                .WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 FIFO link: SaleDetail → PurchaseDetail (optional, 1:N)
            modelBuilder.Entity<SaleDetail>()
                .HasOne(d => d.PurchaseDetail)
                .WithMany(pd => pd.SaleDetails)
                .HasForeignKey(d => d.PurchaseDetailId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Product → StockTransaction (1:N)
            modelBuilder.Entity<StockTransaction>()
                .HasOne(st => st.Product)
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(st => st.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Product → StockSummary (1:1)
            modelBuilder.Entity<StockSummary>()
                .HasKey(s => s.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.StockSummary)
                .WithOne(s => s.Product)
                .HasForeignKey<StockSummary>(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SaleDetailBatch>(entity =>
            {
                entity.HasKey(e => e.SaleDetailBatchId);

                entity.Property(e => e.QuantityUsed).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.SaleDetail)
                    .WithMany(sd => sd.UsedBatches)
                    .HasForeignKey(e => e.SaleDetailId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PurchaseDetail)
                    .WithMany()
                    .HasForeignKey(e => e.PurchaseDetailId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // ============================
            // 💰 Precision Configuration (decimal 18,2)
            // ============================

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.DefaultSalePrice).HasPrecision(18, 4);
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            });

            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 4);
                entity.Property(e => e.UnitCost).HasPrecision(18, 4);
                entity.Property(e => e.TotalCost).HasPrecision(18, 4);
                entity.Property(e => e.RemainingQty).HasPrecision(18, 4);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
                entity.Property(e => e.PaidAmount).HasPrecision(18, 4);
                entity.Property(e => e.BalanceAmount).HasPrecision(18, 4);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.DiscountPercentage).HasPrecision(18, 2);
                entity.Property(e => e.SubTotal).HasPrecision(18, 2);
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalProfit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProfitMargin).HasColumnType("decimal(5,2)");
                entity.Property(e => e.IsFullyPaid).HasDefaultValue(false);
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Profit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NetAmount).HasColumnType("decimal(18,2)");

            });

            modelBuilder.Entity<StockTransaction>()
                .Property(st => st.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockTransaction>()
                .Property(st => st.UnitCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockTransaction>()
                .Property(st => st.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockSummary>()
                .Property(ss => ss.TotalIn)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockSummary>()
                .Property(ss => ss.TotalOut)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockSummary>()
                .Property(ss => ss.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockSummary>()
                .Property(ss => ss.AverageCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockSummary>()
                .Property(ss => ss.TotalValue)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Return>()
                .Property(r => r.TotalRefund)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ReturnDetail>()
                .Property(rd => rd.Quantity)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ReturnDetail>()
                .Property(rd => rd.UnitPrice)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ReturnDetail>()
                .Property(rd => rd.Total)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Tax>()
                .Property(t => t.Rate)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Sale)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Return>()
                .HasMany(r => r.ReturnDetails)
                .WithOne(rd => rd.Return)
                .HasForeignKey(rd => rd.ReturnId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Global Query Filters for Multi-Tenancy
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Sale>().HasQueryFilter(s => s.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Purchase>().HasQueryFilter(pu => pu.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Category>().HasQueryFilter(ca => ca.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Supplier>().HasQueryFilter(su => su.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Customer>().HasQueryFilter(cu => cu.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Return>().HasQueryFilter(re => re.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<Payment>().HasQueryFilter(pa => pa.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<StockSummary>().HasQueryFilter(ss => ss.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<StockTransaction>().HasQueryFilter(st => st.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == this.TenantIdFilter);

            // Details
            modelBuilder.Entity<SaleDetail>().HasQueryFilter(sd => sd.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<PurchaseDetail>().HasQueryFilter(pd => pd.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<ReturnDetail>().HasQueryFilter(rd => rd.TenantId == this.TenantIdFilter);
            modelBuilder.Entity<SaleDetailBatch>().HasQueryFilter(sdb => sdb.TenantId == this.TenantIdFilter);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var tenantId = _tenantProvider?.GetTenantId() ?? 0;
            var userId = _tenantProvider?.GetUserId() ?? 0;

            foreach (var entry in ChangeTracker.Entries<TenantEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Only set TenantId if it's not already set (e.g., set manually during onboarding)
                        if (tenantId > 0 && entry.Entity.TenantId == 0)
                        {
                            entry.Entity.SetTenantId(tenantId);
                        }
                        
                        // Handle audit fields if they exist (Product, Sale, etc. have some)
                        SetAuditFields(entry, userId, true);
                        break;

                    case EntityState.Modified:
                        SetAuditFields(entry, userId, false);
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditFields(EntityEntry entry, int userId, bool isNew)
        {
            // This is a simplified version. Ideally, we'd have a base AuditEntity.
            // But since our entities have different property names (CreatedAt, UpdatedAt), 
            // we'll use a pragmatic approach or reflection.
            
            var now = DateTime.Now;

            // Common patterns in this codebase:
            var createdAtProp = entry.Metadata.FindProperty("CreatedAt");
            if (isNew && createdAtProp != null)
            {
                entry.Property("CreatedAt").CurrentValue = now;
            }

            var updatedAtProp = entry.Metadata.FindProperty("UpdatedAt");
            if (updatedAtProp != null)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }
            
            // Set user IDs if props exist
            var createdByProp = entry.Metadata.FindProperty("CreatedBy");
            if (isNew && createdByProp != null && createdByProp.ClrType == typeof(int) && userId > 0)
            {
                entry.Property("CreatedBy").CurrentValue = userId;
            }
        }
    }
}
