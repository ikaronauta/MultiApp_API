// Controllers/AuthController.cs

using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiApp_API.Data;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs;
using MultiApp_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MultiApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;

    public AuthController(AppDbContext context, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
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

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            var response = new ApiResponse<List<object>>
            {
                Status = "OK",
                Data = new List<object>(),
                Message = string.Empty,
                Error = null
            };

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                response.Message = "Si el usuario existe, se enviará un correo";
                return Ok(response);
            }


            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            user.ResetPasswordToken = token;
            user.ResetPasswordExpires = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            var hostFront = _config["Email:HostFront"];
            var resetLink = $"{hostFront}/reset-password?token={token}";

            var signature = @"
                <br/><br/>
                <table>
                    <tr>
                        <td style='padding-right:20px; border-right:2px solid #ccc;'>
                            <img src='cid:logo' width='300' />
                        </td>
                        <td style='padding-left: 8px;'>
                            <b style='font-family: Arial; font-size: 16px; margin-bottom: 5px;'>MermasAPP</b><br/>
                            <span style='font-family: Calibri; font-size: 14px; margin-bottom: 5px;'>Sistema de Mermas</span><br/><br/>
                            
                            📧 <a style='font-family: Arial; font-size: 12px; margin-bottom: 5px;' href='mailto:mermasapp@epa.com.pa'>mermasapp@epa.com.pa</a><br/>
                            🌐 <a style='font-family: Arial; font-size: 12px; margin-bottom: 5px;' href='https://www.epa.com.pa'>www.epa.com.pa</a>
                        </td>
                    </tr>
                </table>
                ";

            await _emailService.SendEmail(
                user.Email,
                "Recuperación de contraseña",
                $"<span>Recibimos una solicitud para restablecer tu contraseña de <b>MermasAPP</b>. Haz clic en el siguiente enlace para continuar:</span><br/><br/>" +
                $"<a href='{resetLink}'>Restablecer contraseña</a>" + signature
            );

            response.Message = "Si el usuario existe, se enviará un correo";
            return Ok(response);
        }
        catch (Exception ex)
        {

            var errorResponse = new ApiResponse<List<object>>
            {
                Status = "FAIL",
                Data = new List<object>(),
                Message = "No se pudo restablecer la contraseña",
                Error = ex.Message
            };

            return BadRequest(errorResponse);
        }
    }
}