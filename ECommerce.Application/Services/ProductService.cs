using AutoMapper;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products =
            await _productRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product =
            await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductDto dto)
    {
        if (dto.Price <= 0)
        {
            throw new ArgumentException(
                "Product price must be greater than zero.");
        }

        if (dto.StockQuantity < 0)
        {
            throw new ArgumentException(
                "Stock quantity cannot be negative.");
        }

        var skuExists =
            await _productRepository
                .ExistsBySkuAsync(dto.SKU);

        if (skuExists)
        {
            throw new InvalidOperationException(
                $"Product with SKU '{dto.SKU}' already exists.");
        }

        var product =
            _mapper.Map<Product>(dto);

        product.SKU = dto.SKU.ToUpper();

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductDto dto)
    {
        var existingProduct =
            await _productRepository.GetByIdAsync(id);

        if (existingProduct == null)
            return false;

        if (dto.Price <= 0)
        {
            throw new ArgumentException(
                "Price must be positive.");
        }

        _mapper.Map(dto, existingProduct);

        existingProduct.SKU =
            dto.SKU.ToUpper();

        _productRepository.Update(existingProduct);

        await _productRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product =
            await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        _productRepository.Delete(product);

        await _productRepository.SaveChangesAsync();

        return true;
    }
}