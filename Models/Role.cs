// Models/Role.cs

using System.ComponentModel.DataAnnotations;

namespace MultiApp_API.Models;

public class Role
{
    public int Id { get; set; }

    public required string Name { get; set; }

    [MaxLength(255)]
    public required string Description { get; set; } = string.Empty;

    public ICollection<User>? Users { get; set; }
}