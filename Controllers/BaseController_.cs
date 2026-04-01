// Controllers/BaseController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace MultiApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class BaseController : ControllerBase
{
    private readonly AppDbContext _context;

    public BaseController(AppDbContext context)
    {
        _context = context;
    }
}
