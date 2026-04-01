// Controllers/CategoriesController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs.Categories;


namespace MultiApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // Consultar Categorías
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
    {
        try
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedById = c.CreatedById,
                    EditedById = c.EditedById,
                    CreatedDate = c.CreatedDate,
                    EditedDate = c.EditedDate
                })
                .ToListAsync();

            var response = new ApiResponse<List<CategoryDto>>
                {
                    Status = "OK",
                    Data = categories,
                    Message = "Categorías consultadas con éxito",
                    Error = null
                };

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<CategoryDto>>
                {
                    Status = "FAIL",
                    Data = new List<CategoryDto>(),
                    Message = "Error al consultar los usuarios",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }

    // Consultar categoría por ID
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategoryById(int id)
    {
        try
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedById = c.CreatedById,
                    EditedById = c.EditedById,
                    CreatedDate = c.CreatedDate,
                    EditedDate = c.EditedDate
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new ApiResponse<List<CategoryDto>>
                {
                    Status = "FAIL",
                    Data = new List<CategoryDto>(),
                    Message = $"No se encontró categoría con ID {id}",
                    Error = null
                });
            }

            return Ok(new ApiResponse<List<CategoryDto>>
            {
                Status = "OK",
                Data = new List<CategoryDto> { category },
                Message = "Categoría consultada con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<CategoryDto>>
            {
                Status = "FAIL",
                Data = new List<CategoryDto>(),
                Message = "Error al consultar la categoría",
                Error = ex.Message
            });
        }
    }

    // Crear Categoría
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    {
        try
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.Name == dto.Name);

            if (exists)
            {
                var errorResponse = new ApiResponse<List<CategoryDto>>
                {
                    Status = "FAIL",
                    Data = new List<CategoryDto>(),
                    Message = "La categoría ya está registrada",
                    Error = null
                };
            
                return BadRequest(errorResponse);
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedById = dto.CreatedById
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedById = category.CreatedById,
                EditedById = category.EditedById,
                CreatedDate = category.CreatedDate,
                EditedDate = category.EditedDate
            };

            return Ok(new ApiResponse<List<CategoryDto>>
            {
                Status = "OK",
                Data = new List<CategoryDto> { categoryDto },
                Message = "Categoría creada con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<CategoryDto>>
                {
                    Status = "FAIL",
                    Data = new List<CategoryDto>(),
                    Message = "Error al insertar la categoría",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }

    // Editar Categoría
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new ApiResponse<List<CategoryDto>>
                {
                    Status = "FAIL",
                    Data = new List<CategoryDto>(),
                    Message = $"No se encontró categoría con ID {id}",
                    Error = null
                });
            }

            if (category.Name != dto.Name)
            {
                var nameExists = await _context.Categories.AnyAsync(c => c.Name == dto.Name && c.Id != id);
                if (nameExists)
                {
                    return BadRequest(new ApiResponse<List<CategoryDto>>
                    {
                        Status = "FAIL",
                        Data = new List<CategoryDto>(),
                        Message = "El Nombre ya está registrado por otra categoría",
                        Error = null
                    });
                }
            }

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.EditedById = dto.EditedById;

            await _context.SaveChangesAsync();

            var updatedCategoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedById = category.CreatedById,
                EditedById = category.EditedById,
                CreatedDate = category.CreatedDate,
                EditedDate = category.EditedDate
            };

            return Ok(new ApiResponse<List<CategoryDto>>
            {
                Status = "OK",
                Data = new List<CategoryDto> { updatedCategoryDto },
                Message = "Categoría actualizada con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<CategoryDto>>
            {
                Status = "FAIL",
                Data = new List<CategoryDto>(),
                Message = "Error al actualizar la categoría",
                Error = ex.Message
            });
        }
    }

    // Eliminar Categoría
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (category == null)
            {
                return NotFound(new ApiResponse<List<CategoryDto>>
                {
                    Status = "FAIL",
                    Data = new List<CategoryDto>(),
                    Message = $"No se encontró categoría con ID {id}",
                    Error = null
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            var deletedCategoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedById = category.CreatedById,
                EditedById = category.EditedById,
                CreatedDate = category.CreatedDate,
                EditedDate = category.EditedDate
            };

            return Ok(new ApiResponse<List<CategoryDto>>
            {
                Status = "OK",
                Data = new List<CategoryDto> { deletedCategoryDto },
                Message = "Categoría eliminada con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<CategoryDto>>
            {
                Status = "FAIL",
                Data = new List<CategoryDto>(),
                Message = "Error al eliminar la categoría",
                Error = ex.Message
            });
        }
    }
}
