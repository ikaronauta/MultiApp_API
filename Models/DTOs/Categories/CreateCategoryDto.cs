// Models/DTOs/CreateCategoryDto.cs

using System.ComponentModel.DataAnnotations;

namespace MultiApp_API.Models.DTOs.Categories;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public int CreatedById { get; set; }

}
