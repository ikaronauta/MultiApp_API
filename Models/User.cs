// Models/User.cs

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
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserStatus Status { get; set; } = UserStatus.Activo;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}
