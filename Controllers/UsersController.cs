// Controllers/UsersController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs.Users;
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

    // Consultar Usuarios
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
                    Status = u.Status,
                    RoleId = u.RoleId,
                    CreatedById = u.CreatedById,
                    EditedById = u.EditedById,
                    CreatedDate = u.CreatedDate,
                    EditedDate = u.EditedDate
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

    // Consultar usuario por ID
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUserById(int id)
    {
        try
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    DocumentType = u.DocumentType,
                    DocumentNumber = u.DocumentNumber,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    BirthDate = u.BirthDate,
                    Status = u.Status,
                    RoleId = u.RoleId,
                    CreatedById = u.CreatedById,
                    EditedById = u.EditedById,
                    CreatedDate = u.CreatedDate,
                    EditedDate = u.EditedDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = $"No se encontró usuario con ID {id}",
                    Error = null
                });
            }

            return Ok(new ApiResponse<List<UserDto>>
            {
                Status = "OK",
                Data = new List<UserDto> { user },
                Message = "Usuario consultado con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<UserDto>>
            {
                Status = "FAIL",
                Data = new List<UserDto>(),
                Message = "Error al consultar el usuario",
                Error = ex.Message
            });
        }
    }

    // Crear Usuario
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        try
        {
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

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                DocumentType = dto.DocumentType,
                DocumentNumber = dto.DocumentNumber,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                PasswordHash = passwordHash,
                Status = UserStatus.Activo,
                RoleId = dto.RoleId,
                CreatedById = dto.CreatedById
            };

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
                Status = user.Status,
                RoleId = user.RoleId,
                CreatedById = user.CreatedById,
                EditedById = user.EditedById,
                CreatedDate = user.CreatedDate,
                EditedDate = user.EditedDate
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

    // Editar Usuario
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> UpdateUser(int id, UpdateUserDto dto)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = $"No se encontró usuario con ID {id}",
                    Error = null
                });
            }

            if (user.Email != dto.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
                if (emailExists)
                {
                    return BadRequest(new ApiResponse<List<UserDto>>
                    {
                        Status = "FAIL",
                        Data = new List<UserDto>(),
                        Message = "El email ya está registrado por otro usuario",
                        Error = null
                    });
                }
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists)
            {
                return BadRequest(new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = "El rol no existe",
                    Error = null
                });
            }

            user.DocumentType = dto.DocumentType;
            user.DocumentNumber = dto.DocumentNumber;
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.BirthDate = dto.BirthDate;
            user.RoleId = dto.RoleId;
            user.EditedById = dto.EditedById;

            await _context.SaveChangesAsync();

            var updatedUserDto = new UserDto
            {
                Id = user.Id,
                DocumentType = user.DocumentType,
                DocumentNumber = user.DocumentNumber,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Status = user.Status,
                RoleId = user.RoleId,
                CreatedById = user.CreatedById,
                EditedById = user.EditedById,
                CreatedDate = user.CreatedDate,
                EditedDate = user.EditedDate
            };

            return Ok(new ApiResponse<List<UserDto>>
            {
                Status = "OK",
                Data = new List<UserDto> { updatedUserDto },
                Message = "Usuario actualizado con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<UserDto>>
            {
                Status = "FAIL",
                Data = new List<UserDto>(),
                Message = "Error al actualizar el usuario",
                Error = ex.Message
            });
        }
    }

    // Eliminar Usuario
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse<List<UserDto>>
                {
                    Status = "FAIL",
                    Data = new List<UserDto>(),
                    Message = $"No se encontró usuario con ID {id}",
                    Error = null
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            var deletedUserDto = new UserDto
            {
                Id = user.Id,
                DocumentType = user.DocumentType,
                DocumentNumber = user.DocumentNumber,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Status = user.Status,
                RoleId = user.RoleId,
                CreatedById = user.CreatedById,
                EditedById = user.EditedById,
                CreatedDate = user.CreatedDate,
                EditedDate = user.EditedDate
            };

            return Ok(new ApiResponse<List<UserDto>>
            {
                Status = "OK",
                Data = new List<UserDto> { deletedUserDto },
                Message = "Usuario eliminado con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<UserDto>>
            {
                Status = "FAIL",
                Data = new List<UserDto>(),
                Message = "Error al eliminar el usuario",
                Error = ex.Message
            });
        }
    }
    
}
