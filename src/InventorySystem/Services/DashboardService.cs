using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services;

public record DashboardStats(
    int ProductCount,
    int LowStockCount,
    decimal StockValue,
    int PurchaseTodayCount,
    int SalesTodayCount,
    decimal SalesTodayAmount);

/// <summary>仪表盘汇总数据。</summary>
public class DashboardService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public DashboardService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task<DashboardStats> GetStatsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var today = DateTime.Today;

        var productCount = await db.Products.CountAsync();
        var lowStock = await db.Products
            .CountAsync(p => p.IsActive && p.SafetyStock > 0 && p.StockQuantity <= p.SafetyStock);
        var stockValue = await db.Products
            .SumAsync(p => (decimal?)(p.StockQuantity * p.PurchasePrice)) ?? 0m;

        var purchaseToday = await db.PurchaseOrders.CountAsync(o => o.OrderDate == today);
        var salesToday = await db.SalesOrders.Where(o => o.OrderDate == today).ToListAsync();

        return new DashboardStats(
            productCount,
            lowStock,
            stockValue,
            purchaseToday,
            salesToday.Count,
            salesToday.Where(o => o.Status == OrderStatus.Confirmed).Sum(o => o.TotalAmount));
    }
}
