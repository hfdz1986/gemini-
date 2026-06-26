using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

/// <summary>客户</summary>
public class Customer : BaseEntity
{
    [Required(ErrorMessage = "客户名称必填")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Contact { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(200)]
    public string? Remark { get; set; }
}
