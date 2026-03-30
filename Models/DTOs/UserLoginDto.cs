// Models/DTOs/UserLoginDto.cs

namespace MultiApp_API.Models.DTOs
{
    public class UserLoginDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}