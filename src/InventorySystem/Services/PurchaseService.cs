using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services;

/// <summary>采购（进货）业务服务。</summary>
public class PurchaseService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public PurchaseService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task<List<PurchaseOrder>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.PurchaseOrders
            .Include(o => o.Supplier)
            .OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id)
            .AsNoTracking().ToListAsync();
    }

    public async Task<PurchaseOrder?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.PurchaseOrders
            .Include(o => o.Supplier)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>创建草稿采购单。</summary>
    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder order)
    {
        await using var db = await _factory.CreateDbContextAsync();
        order.Status = OrderStatus.Draft;
        order.OrderNo = await NextNoAsync(db);
        order.TotalAmount = order.Items.Sum(i => i.Amount);
        db.PurchaseOrders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }

    /// <summary>确认采购单 → 商品入库并生成库存流水。整个过程在一个事务中完成。</summary>
    public async Task ConfirmAsync(int orderId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        var order = await db.PurchaseOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new InvalidOperationException("采购单不存在");

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("只有草稿状态的单据可以确认");
        if (order.Items.Count == 0)
            throw new InvalidOperationException("采购单没有明细，无法确认");

        foreach (var item in order.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId)
                ?? throw new InvalidOperationException("商品不存在");
            InventoryService.Apply(db, product, MovementType.In,
                item.Quantity, "采购单", order.OrderNo);
        }

        order.Status = OrderStatus.Confirmed;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    /// <summary>取消草稿采购单（已确认的单据不支持直接取消）。</summary>
    public async Task DeleteDraftAsync(int orderId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.PurchaseOrders.FindAsync(orderId);
        if (order is null) return;
        if (order.Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("已确认的采购单不能删除");
        db.PurchaseOrders.Remove(order);
        await db.SaveChangesAsync();
    }

    private static async Task<string> NextNoAsync(AppDbContext db)
    {
        var today = DateTime.Today;
        var prefix = $"PO{today:yyyyMMdd}";
        var countToday = await db.PurchaseOrders.CountAsync(o => o.OrderNo.StartsWith(prefix));
        return $"{prefix}-{countToday + 1:D3}";
    }
}
