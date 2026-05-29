using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get all products", Description = "Retrieve paginated list of products with optional filtering")]
    [SwaggerResponse(200, "List of products", typeof(PagedResult<ProductDto>))]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null)
    {
        var result = await _productService.GetAllProductsAsync(pageNumber, pageSize, search, category);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get product by ID", Description = "Retrieve a specific product by its unique identifier")]
    [SwaggerResponse(200, "Product found", typeof(ProductDto))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Create product", Description = "Create a new product (Admin only)")]
    [SwaggerResponse(201, "Product created", typeof(ProductDto))]
    [SwaggerResponse(400, "Invalid input data")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Update product", Description = "Update an existing product (Admin only)")]
    [SwaggerResponse(200, "Product updated", typeof(ProductDto))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productService.UpdateProductAsync(id, dto);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete product", Description = "Delete a product (Admin only)")]
    [SwaggerResponse(204, "Product deleted")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get categories", Description = "Retrieve list of all product categories")]
    [SwaggerResponse(200, "List of categories", typeof(IEnumerable<string>))]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await _productService.GetCategoriesAsync();
        return Ok(categories);
    }
}
