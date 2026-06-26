using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

/// <summary>商品分类</summary>
public class Category : BaseEntity
{
    [Required(ErrorMessage = "分类名称必填")]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Remark { get; set; }

    public List<Product> Products { get; set; } = new();
}
