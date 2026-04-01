// Models/DTOs/Users/CreateUserDto.cs

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace MultiApp_API.Models.DTOs.Users;

public class CreateUserDto
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
    public string Password { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public int CreatedById { get; set; }
}