using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // 计算属性 Amount 不持久化
        b.Entity<PurchaseOrderItem>().Ignore(i => i.Amount);
        b.Entity<SalesOrderItem>().Ignore(i => i.Amount);

        b.Entity<Product>().HasIndex(p => p.Code).IsUnique();

        // 金额精度
        foreach (var p in new[]
        {
            b.Entity<Product>().Property(x => x.PurchasePrice),
            b.Entity<Product>().Property(x => x.SalePrice),
            b.Entity<PurchaseOrder>().Property(x => x.TotalAmount),
            b.Entity<SalesOrder>().Property(x => x.TotalAmount),
            b.Entity<PurchaseOrderItem>().Property(x => x.UnitPrice),
            b.Entity<SalesOrderItem>().Property(x => x.UnitPrice),
        })
        {
            p.HasColumnType("decimal(18,2)");
        }

        // 删除主表时级联删除明细
        b.Entity<PurchaseOrder>()
            .HasMany(o => o.Items).WithOne(i => i.PurchaseOrder!)
            .HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<SalesOrder>()
            .HasMany(o => o.Items).WithOne(i => i.SalesOrder!)
            .HasForeignKey(i => i.SalesOrderId).OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(b);
    }

    /// <summary>统一在保存时维护 UpdatedAt 时间戳。</summary>
    public override int SaveChanges()
    {
        Stamp();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        Stamp();
        return base.SaveChangesAsync(ct);
    }

    private void Stamp()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.Now;
        }
    }
}
