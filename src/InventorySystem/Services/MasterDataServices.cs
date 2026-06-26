using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services;

/// <summary>分类服务。</summary>
public class CategoryService : CrudService<Category>
{
    public CategoryService(IDbContextFactory<AppDbContext> f) : base(f) { }
}

/// <summary>供应商服务。</summary>
public class SupplierService : CrudService<Supplier>
{
    public SupplierService(IDbContextFactory<AppDbContext> f) : base(f) { }
}

/// <summary>客户服务。</summary>
public class CustomerService : CrudService<Customer>
{
    public CustomerService(IDbContextFactory<AppDbContext> f) : base(f) { }
}

/// <summary>商品服务，列表预加载分类。</summary>
public class ProductService : CrudService<Product>
{
    public ProductService(IDbContextFactory<AppDbContext> f) : base(f) { }

    protected override IQueryable<Product> Query(AppDbContext db)
        => db.Products.Include(p => p.Category);

    /// <summary>低于安全库存的商品。</summary>
    public async Task<List<Product>> GetLowStockAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Products
            .Where(p => p.IsActive && p.SafetyStock > 0 && p.StockQuantity <= p.SafetyStock)
            .OrderBy(p => p.StockQuantity)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>可下拉选择的启用商品。</summary>
    public async Task<List<Product>> GetActiveAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Products.Where(p => p.IsActive)
            .OrderBy(p => p.Code).AsNoTracking().ToListAsync();
    }
}
