using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

/// <summary>商品 / 物料</summary>
public class Product : BaseEntity
{
    [Required(ErrorMessage = "商品编码必填")]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "商品名称必填")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    /// <summary>计量单位（个 / 件 / 箱 ...）</summary>
    [StringLength(20)]
    public string Unit { get; set; } = "个";

    /// <summary>参考进价</summary>
    [Range(0, double.MaxValue, ErrorMessage = "进价不能为负")]
    public decimal PurchasePrice { get; set; }

    /// <summary>参考售价</summary>
    [Range(0, double.MaxValue, ErrorMessage = "售价不能为负")]
    public decimal SalePrice { get; set; }

    /// <summary>当前库存数量（由库存流水汇总维护，请勿手工随意修改）</summary>
    public int StockQuantity { get; set; }

    /// <summary>安全库存（低于此值在仪表盘预警）</summary>
    public int SafetyStock { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(200)]
    public string? Remark { get; set; }
}
