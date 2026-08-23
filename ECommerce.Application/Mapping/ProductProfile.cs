using AutoMapper;
using ECommerce.Application.DTOs.Product;

using ECommerce.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Application.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<CreateProductDto, Product>();

        CreateMap<UpdateProductDto, Product>();
    }
}