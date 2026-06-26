using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

/// <summary>库存流水（每一次出入库的不可变记录）</summary>
public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public MovementType Type { get; set; }

    /// <summary>变动数量（始终为正数，方向由 Type 决定）</summary>
    public int Quantity { get; set; }

    /// <summary>变动后的结存数量</summary>
    public int BalanceAfter { get; set; }

    /// <summary>来源单据类型，如 采购单 / 销售单 / 盘点</summary>
    [StringLength(20)]
    public string SourceType { get; set; } = string.Empty;

    /// <summary>来源单据编号</summary>
    [StringLength(30)]
    public string? SourceNo { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.Now;

    [StringLength(200)]
    public string? Remark { get; set; }
}
