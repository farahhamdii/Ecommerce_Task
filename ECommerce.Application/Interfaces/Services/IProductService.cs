using ECommerce.Application.DTOs.Product;

namespace ECommerce.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();

    Task<ProductDto?> GetByIdAsync(int id);

    Task<ProductDto> CreateAsync(CreateProductDto dto);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductDto dto);

    Task<bool> DeleteAsync(int id);
}