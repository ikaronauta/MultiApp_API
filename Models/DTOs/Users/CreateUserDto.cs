// Models/DTOs/Users/CreateUserDto.cs

using System.Text.Json.Serialization;

namespace MultiApp_API.Models.DTOs.Users;

public class CreateUserDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DocumentType DocumentType { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    public string Password { get; set; } = string.Empty;

    public int RoleId { get; set; }
}