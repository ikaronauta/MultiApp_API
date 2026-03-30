// Models/User.cs

namespace MultiApp_API.Models;

public class User 
{
    public int Id { get; set; }

    public required DocumentType DocumentType { get; set; }

    public required string DocumentNumber { get; set; }

    public required string Email { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required DateOnly BirthDate { get; set; }

    public required string PasswordHash { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Activo;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}
