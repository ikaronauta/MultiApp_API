// Controllers/UsersController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using MultiApp_API.Models;

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

    [HttpGet]
    public IEnumerable<Users> Get()
    {
        return _context.Users.ToList();
    }
}
