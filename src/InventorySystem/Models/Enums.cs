namespace InventorySystem.Models;

/// <summary>单据状态</summary>
public enum OrderStatus
{
    /// <summary>草稿（未过账，不影响库存）</summary>
    Draft = 0,
    /// <summary>已确认（已过账，已影响库存）</summary>
    Confirmed = 1,
    /// <summary>已取消</summary>
    Cancelled = 2
}

/// <summary>库存变动类型</summary>
public enum MovementType
{
    /// <summary>入库（采购、退货入库等）</summary>
    In = 0,
    /// <summary>出库（销售、领用出库等）</summary>
    Out = 1,
    /// <summary>盘点调整</summary>
    Adjust = 2
}
