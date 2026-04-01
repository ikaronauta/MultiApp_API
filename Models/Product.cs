// Models/Product.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiApp_API.Models;

public class Product 
{
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public int Stock { get; set; } = 0;

    [Required]
    public int MinStock { get; set; } = 5;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public int CategoryId { get; set;}

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public int? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public int? EditedById { get; set; }

    [ForeignKey("EditedById")]
    public User? EditedBy { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? EditedDate { get; set; }
}
