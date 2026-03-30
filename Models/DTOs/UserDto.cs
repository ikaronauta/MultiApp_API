// Models/DTOs/UserDto.cs

namespace MultiApp_API.Models.DTOs;

public class UserDto
    {
        public int Id { get; set; }

        public required DocumentType DocumentType { get; set; }

        public required  string DocumentNumber { get; set; }

        public required  string Email { get; set; }

        public required  string FirstName { get; set; }

        public required  string LastName { get; set; }

        public required  DateOnly BirthDate { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Activo;
    }