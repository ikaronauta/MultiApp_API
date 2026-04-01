// Models/User.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiApp_API.Models;

public class User 
{
    public int Id { get; set; }

    [Required]
    public DocumentType DocumentType { get; set; }

    [Required]
    [MaxLength(50)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserStatus Status { get; set; } = UserStatus.Activo;

    [Required]
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public int? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public int? EditedById { get; set; }

    [ForeignKey("EditedById")]
    public User? EditedBy { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? EditedDate { get; set; }

     [InverseProperty("CreatedBy")]
    public ICollection<User> CreatedUsers { get; set; } = new List<User>();

    [InverseProperty("EditedBy")]
    public ICollection<User> EditedUsers { get; set; } = new List<User>();

    [InverseProperty("CreatedBy")]
    public ICollection<Category> CreatedCategories { get; set; } = new List<Category>();

    [InverseProperty("EditedBy")]
    public ICollection<Category> EditedCategories { get; set; } = new List<Category>();

    [InverseProperty("CreatedBy")]
    public ICollection<Product> CreatedProducts { get; set; } = new List<Product>();

    [InverseProperty("EditedBy")]
    public ICollection<Product> EditedProducts { get; set; } = new List<Product>();
}
