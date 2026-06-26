namespace InventorySystem.Models;

/// <summary>
/// 所有实体的基类。框架中的通用 CRUD 服务依赖此基类提供的 Id 主键。
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>最后更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
}
