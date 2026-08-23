using ECommerce.Application.DTOs.Customer;

namespace ECommerce.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerDetailsDto?> GetByIdAsync(int id);

    Task<CustomerDto> CreateAsync(
        CreateCustomerDto dto);

    Task<bool> UpgradeToVipAsync(int id);
}