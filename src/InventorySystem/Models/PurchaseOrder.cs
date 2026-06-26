using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

/// <summary>采购单（进货）</summary>
public class PurchaseOrder : BaseEntity
{
    /// <summary>单据编号，保存时自动生成</summary>
    [StringLength(30)]
    public string OrderNo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "请选择供应商")]
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Today;

    public OrderStatus Status { get; set; } = OrderStatus.Draft;

    /// <summary>合计金额（由明细汇总）</summary>
    public decimal TotalAmount { get; set; }

    [StringLength(200)]
    public string? Remark { get; set; }

    public List<PurchaseOrderItem> Items { get; set; } = new();
}

/// <summary>采购单明细</summary>
public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    [Required(ErrorMessage = "请选择商品")]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于 0")]
    public int Quantity { get; set; } = 1;

    [Range(0, double.MaxValue, ErrorMessage = "单价不能为负")]
    public decimal UnitPrice { get; set; }

    /// <summary>小计 = 数量 × 单价</summary>
    public decimal Amount => Quantity * UnitPrice;
}
