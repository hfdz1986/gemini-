using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Data;

/// <summary>应用启动时确保数据库已创建，并写入演示数据。</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        await db.Database.EnsureCreatedAsync();

        if (await db.Products.AnyAsync())
            return; // 已有数据，跳过

        var cat1 = new Category { Name = "饮料", Remark = "瓶装、罐装饮品" };
        var cat2 = new Category { Name = "零食", Remark = "膨化、坚果等" };
        db.Categories.AddRange(cat1, cat2);

        db.Suppliers.AddRange(
            new Supplier { Name = "华东食品批发", Contact = "张经理", Phone = "13800000001", Address = "上海市浦东新区" },
            new Supplier { Name = "南方饮料供应链", Contact = "李主管", Phone = "13900000002", Address = "广州市天河区" });

        db.Customers.AddRange(
            new Customer { Name = "城西便利店", Contact = "王老板", Phone = "13700000003", Address = "杭州市西湖区" },
            new Customer { Name = "学府路超市", Contact = "赵店长", Phone = "13600000004", Address = "南京市鼓楼区" });

        db.Products.AddRange(
            new Product { Code = "P001", Name = "矿泉水 550ml", Category = cat1, Unit = "瓶", PurchasePrice = 0.8m, SalePrice = 2.0m, StockQuantity = 0, SafetyStock = 50 },
            new Product { Code = "P002", Name = "可乐 330ml", Category = cat1, Unit = "罐", PurchasePrice = 1.5m, SalePrice = 3.5m, StockQuantity = 0, SafetyStock = 40 },
            new Product { Code = "P003", Name = "薯片 大包", Category = cat2, Unit = "袋", PurchasePrice = 3.2m, SalePrice = 6.5m, StockQuantity = 0, SafetyStock = 30 },
            new Product { Code = "P004", Name = "混合坚果 罐装", Category = cat2, Unit = "罐", PurchasePrice = 12.0m, SalePrice = 22.0m, StockQuantity = 0, SafetyStock = 20 });

        await db.SaveChangesAsync();
    }
}
