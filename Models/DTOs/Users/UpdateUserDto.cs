// Models/DTOs/Users/UpdateUserDto.cs

using System.Text.Json.Serialization;

namespace MultiApp_API.Models.DTOs.Users;

public class UpdateUserDto
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

    public int EditedById { get; set; }
}