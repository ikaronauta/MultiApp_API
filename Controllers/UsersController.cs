// Controllers/UsersController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace MultiApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsers()
    {
        try
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    DocumentType = u.DocumentType,
                    DocumentNumber = u.DocumentNumber,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    BirthDate = u.BirthDate,
                    Status = u.Status
                })
                .ToListAsync();

            var response = new ApiResponse<List<UserDto>>
                {
                    Status = "OK",
                    Data = users,
                    Message = "Usuarios consultados con éxito",
                    Error = null
                };

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = "Error al consultar los usuarios",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        try
        {
            // 🔴 Validar si el email ya existe
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
            {
                var errorResponse = new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = "El email ya está registrado",
                    Error = null
                };
            
                return BadRequest(errorResponse);
            }

            // 🔴 Validar si el rol existe
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == dto.RoleId);

            if (!roleExists)
            {
                var errorResponse = new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = "El rol no existe",
                    Error = null
                };
            
                return BadRequest(errorResponse);
            }

            // 🔐 Hashear contraseña
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 🧱 Crear entidad
            var user = new User
            {
                DocumentType = dto.DocumentType,
                DocumentNumber = dto.DocumentNumber,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                PasswordHash = passwordHash,
                RoleId = dto.RoleId,
                Status = UserStatus.Activo
            };

            // 💾 Guardar
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                DocumentType = user.DocumentType,
                DocumentNumber = user.DocumentNumber,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Status = user.Status
            };

            return Ok(new ApiResponse<UserDto>
            {
                Status = "OK",
                Data = userDto,
                Message = "Usuario creado con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = "Error al insertar el usuario",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }
}
