// Models/Category.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiApp_API.Models;

public class Category 
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public int CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public int? EditedById { get; set; }

    [ForeignKey("EditedById")]
    public User? EditedBy { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime? EditedDate { get; set; }

}
