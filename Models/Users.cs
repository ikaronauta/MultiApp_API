// Models/Users.cs

namespace MultiApp_API.Models;

public enum DocumentType
{
    CC,
    NIT,
    Passport
}

public class Users 
{
    public int Id { get; set; }

    public required DocumentType DocumentType { get; set; }

    public required string DocumentNumber { get; set; }

    public required string Email { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public DateOnly BirthDate { get; set; }

    public required string PasswordHash { get; set; }
}
