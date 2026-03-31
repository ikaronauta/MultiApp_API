// Controllers/AuthController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MultiApp_API.Data;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;

namespace MultiApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API está funcionando ✅");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = new ApiResponse<List<UserLoginDto>>
            {
                Status = string.Empty,
                Data = new List<UserLoginDto>(),
                Message = string.Empty,
                Error = null
            };

            // Buscar el usuario por email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null){
                response.Message = "Usuario o contraseña incorrectos";
                return Unauthorized(response);
            }
                
            // Verificar contraseña
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValid)
            {
                response.Message = "Usuario o contraseña incorrectos";
                return Unauthorized(response);
            }
                
            // Lógica JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyString = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key no está configurada");

            var key = Encoding.UTF8.GetBytes(keyString);

            var timeExpireString = _config["Jwt:ExpiresInMinutes"];

            if (string.IsNullOrEmpty(timeExpireString))
                throw new Exception("Expires no está configurada");

            int timeExpire = int.Parse(timeExpireString);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
                }),
                Expires = DateTime.UtcNow.AddMinutes(timeExpire),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            response.Status = "OK";
            response.Data.Add(new UserLoginDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = tokenString
            });
            response.Message = "Inicio de sesión exitoso";
            response.Error = null;

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<UserLoginDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserLoginDto>(),
                    Message = "No se pudo iniciar sesión",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }
}