// Models/DTOs/ProductDto.cs

using System.ComponentModel.DataAnnotations;

namespace MultiApp_API.Models.DTOs.Products;

public class ProductDto
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

    public int? CreatedById { get; set; }

    public int? EditedById { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime? EditedDate { get; set; }

}
