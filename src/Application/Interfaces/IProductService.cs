using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? category);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto);
    Task DeleteProductAsync(Guid id);
    Task<bool> ProductExistsAsync(Guid id);
    Task<IEnumerable<string>> GetCategoriesAsync();
}
