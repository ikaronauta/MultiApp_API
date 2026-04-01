// Models/DTOs/CategoryDto.cs

using System.ComponentModel.DataAnnotations;

namespace MultiApp_API.Models.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public int? CreatedById { get; set; }

    public int? EditedById { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime? EditedDate { get; set; }

}
