using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services;

/// <summary>
/// 库存核心服务：所有库存数量的变化都必须经过这里，
/// 以保证 Product.StockQuantity 与 StockMovement 流水始终一致。
/// </summary>
public class InventoryService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public InventoryService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    /// <summary>
    /// 在给定的 DbContext 内对单个商品过账（不单独提交，由调用方统一 SaveChanges）。
    /// 这样采购/销售可以把多条明细放在同一事务中。
    /// </summary>
    public static StockMovement Apply(AppDbContext db, Product product, MovementType type,
        int quantity, string sourceType, string? sourceNo, string? remark = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("数量必须大于 0", nameof(quantity));

        var delta = type == MovementType.Out ? -quantity : quantity;
        var newBalance = product.StockQuantity + delta;
        if (newBalance < 0)
            throw new InvalidOperationException($"库存不足：{product.Name} 当前 {product.StockQuantity}，需出库 {quantity}");

        product.StockQuantity = newBalance;

        var movement = new StockMovement
        {
            ProductId = product.Id,
            Type = type,
            Quantity = quantity,
            BalanceAfter = newBalance,
            SourceType = sourceType,
            SourceNo = sourceNo,
            Remark = remark,
            OccurredAt = DateTime.Now
        };
        db.StockMovements.Add(movement);
        return movement;
    }

    /// <summary>手工库存调整（盘盈/盘亏），独立提交。</summary>
    public async Task AdjustAsync(int productId, int targetQuantity, string? remark)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var product = await db.Products.FindAsync(productId)
            ?? throw new InvalidOperationException("商品不存在");

        var diff = targetQuantity - product.StockQuantity;
        if (diff == 0) return;

        var movement = Apply(db, product,
            diff > 0 ? MovementType.In : MovementType.Out,
            Math.Abs(diff),
            "盘点", null, remark ?? "库存调整");

        // 盘点统一记为 Adjust 类型，便于流水区分
        movement.Type = MovementType.Adjust;

        await db.SaveChangesAsync();
    }

    /// <summary>库存流水（可按商品过滤），最新在前。</summary>
    public async Task<List<StockMovement>> GetMovementsAsync(int? productId = null, int take = 200)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var q = db.StockMovements.Include(m => m.Product).AsNoTracking();
        if (productId is not null)
            q = q.Where(m => m.ProductId == productId);
        return await q.OrderByDescending(m => m.OccurredAt).Take(take).ToListAsync();
    }
}
