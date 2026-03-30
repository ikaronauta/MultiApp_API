// Controllers/UsersController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
}
