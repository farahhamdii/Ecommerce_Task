using AutoMapper;
using ECommerce.Application.DTOs.Customer;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>();

        CreateMap<CreateCustomerDto, Customer>();

        CreateMap<Customer, CustomerDetailsDto>();
    }
}