// Controllers/ProductsController.cs

using Microsoft.AspNetCore.Mvc;
using MultiApp_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MultiApp_API.Models;
using MultiApp_API.Models.DTOs.Products;


namespace MultiApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // Consultar Productos
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts()
    {
        try
        {
            var products = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    Description = p.Description,
                    Stock = p.Stock,
                    MinStock = p.MinStock,
                    CreatedById = p.CreatedById,
                    IsActive = p.IsActive,
                    EditedById = p.EditedById,
                    CreatedDate = p.CreatedDate,
                    EditedDate = p.EditedDate
                })
                .ToListAsync();

            var response = new ApiResponse<List<ProductDto>>
                {
                    Status = "OK",
                    Data = products,
                    Message = "Productos consultados con éxito",
                    Error = null
                };

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<ProductDto>>
                {
                    Status = "FAIL",
                    Data = new List<ProductDto>(),
                    Message = "Error al consultar los productos",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }

    // Consultar producto por ID
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProductById(int id)
    {
        try
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    Description = p.Description,
                    Stock = p.Stock,
                    MinStock = p.MinStock,
                    CreatedById = p.CreatedById,
                    IsActive = p.IsActive,
                    EditedById = p.EditedById,
                    CreatedDate = p.CreatedDate,
                    EditedDate = p.EditedDate
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new ApiResponse<List<ProductDto>>
                {
                    Status = "FAIL",
                    Data = new List<ProductDto>(),
                    Message = $"No se encontró producto con ID {id}",
                    Error = null
                });
            }

            return Ok(new ApiResponse<List<ProductDto>>
            {
                Status = "OK",
                Data = new List<ProductDto> { product },
                Message = "Producto consultado con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<ProductDto>>
            {
                Status = "FAIL",
                Data = new List<ProductDto>(),
                Message = "Error al consultar el producto",
                Error = ex.Message
            });
        }
    }

    // Crear Producto
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        try
        {
            var exists = await _context.Products
                .AnyAsync(c => c.Name == dto.Name);

            if (exists)
            {
                var errorResponse = new ApiResponse<List<ProductDto>>
                {
                    Status = "FAIL",
                    Data = new List<ProductDto>(),
                    Message = "El producto ya está registrada",
                    Error = null
                };
            
                return BadRequest(errorResponse);
            }

            var product = new Product
            {
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Stock = dto.Stock,
                MinStock = dto.MinStock,       
                IsActive = dto.IsActive,
                CategoryId = dto.CategoryId,
                CreatedById = dto.CreatedById
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                SKU = product.SKU,
                Name = product.Name,
                Description = product.Description,
                Stock = product.Stock,
                MinStock = product.MinStock,
                IsActive = product.IsActive,
                CreatedById = product.CreatedById,
                EditedById = product.EditedById,
                CreatedDate = product.CreatedDate,
                EditedDate = product.EditedDate
            };

            return Ok(new ApiResponse<List<ProductDto>>
            {
                Status = "OK",
                Data = new List<ProductDto> { productDto },
                Message = "Producto creado con éxito",
                Error = null
            });
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<List<ProductDto>>
                {
                    Status = "FAIL",
                    Data = new List<ProductDto>(),
                    Message = "Error al insertar el producto",
                    Error = ex.Message
                };
            
            return BadRequest(errorResponse);
        }
    }

}
