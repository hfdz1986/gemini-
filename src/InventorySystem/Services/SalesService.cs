using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services;

/// <summary>销售（出货）业务服务。</summary>
public class SalesService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public SalesService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task<List<SalesOrder>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.SalesOrders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id)
            .AsNoTracking().ToListAsync();
    }

    public async Task<SalesOrder?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.SalesOrders
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>创建草稿销售单。</summary>
    public async Task<SalesOrder> CreateAsync(SalesOrder order)
    {
        await using var db = await _factory.CreateDbContextAsync();
        order.Status = OrderStatus.Draft;
        order.OrderNo = await NextNoAsync(db);
        order.TotalAmount = order.Items.Sum(i => i.Amount);
        db.SalesOrders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }

    /// <summary>确认销售单 → 商品出库并生成库存流水（库存不足会抛错并回滚）。</summary>
    public async Task ConfirmAsync(int orderId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        var order = await db.SalesOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new InvalidOperationException("销售单不存在");

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("只有草稿状态的单据可以确认");
        if (order.Items.Count == 0)
            throw new InvalidOperationException("销售单没有明细，无法确认");

        foreach (var item in order.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId)
                ?? throw new InvalidOperationException("商品不存在");
            // 库存不足时 Apply 抛异常，事务整体回滚
            InventoryService.Apply(db, product, MovementType.Out,
                item.Quantity, "销售单", order.OrderNo);
        }

        order.Status = OrderStatus.Confirmed;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task DeleteDraftAsync(int orderId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.SalesOrders.FindAsync(orderId);
        if (order is null) return;
        if (order.Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("已确认的销售单不能删除");
        db.SalesOrders.Remove(order);
        await db.SaveChangesAsync();
    }

    private static async Task<string> NextNoAsync(AppDbContext db)
    {
        var today = DateTime.Today;
        var prefix = $"SO{today:yyyyMMdd}";
        var countToday = await db.SalesOrders.CountAsync(o => o.OrderNo.StartsWith(prefix));
        return $"{prefix}-{countToday + 1:D3}";
    }
}
