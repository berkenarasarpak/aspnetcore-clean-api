using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductDto>> GetAllProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? category)
    {
        var query = await _unitOfWork.Repository<Product>().GetAllAsync();
        var products = query.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            products = products.Where(p => 
                p.Name.ToLower().Contains(term) || 
                p.Description.ToLower().Contains(term) ||
                p.Sku.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            products = products.Where(p => p.Category == category);
        }

        var totalCount = products.Count();
        var items = products
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = items.Select(MapToProductDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        return product != null ? MapToProductDto(product) : null;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var exists = await _unitOfWork.Repository<Product>()
            .ExistsAsync(p => p.Sku == dto.Sku);

        if (exists)
        {
            throw new InvalidOperationException("Product with this SKU already exists");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Sku = dto.Sku,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Product>().AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToProductDto(product);
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Product>().UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToProductDto(product);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        await _unitOfWork.Repository<Product>().DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ProductExistsAsync(Guid id)
    {
        return await _unitOfWork.Repository<Product>().ExistsAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        return products.Select(p => p.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c);
    }

    private static ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
