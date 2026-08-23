using AutoMapper;
using ECommerce.Application.DTOs.Customer;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CustomerDetailsDto?> GetByIdAsync(int id)
    {
        var customer =
            await _customerRepository
                .GetByIdWithOrdersAsync(id);

        if (customer == null)
            return null;

        return _mapper.Map<CustomerDetailsDto>(customer);
    }

    public async Task<CustomerDto> CreateAsync(
        CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            throw new ArgumentException(
                "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email) ||
            !dto.Email.Contains("@"))
        {
            throw new ArgumentException(
                "A valid email address is required.");
        }

        var emailExists =
            await _customerRepository
                .ExistsByEmailAsync(dto.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email is already registered.");
        }

        var customer =
            _mapper.Map<ECommerce.Domain.Entities.Customer>(dto);

        await _customerRepository.AddAsync(customer);

        await _customerRepository.SaveChangesAsync();

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<bool> UpgradeToVipAsync(int id)
    {
        var customer =
            await _customerRepository
                .GetByIdWithOrdersAsync(id);

        if (customer == null)
            return false;

        var totalSpent = customer.Orders
            .Where(o => o.Status == OrderStatus.Paid)
            .Sum(o => o.TotalAmount);

        if (totalSpent < 500m)
        {
            throw new InvalidOperationException(
                $"Customer does not qualify for VIP. " +
                $"Total spend {totalSpent:C} is less than required $500.00");
        }

        customer.IsVip = true;

        _customerRepository.Update(customer);

        await _customerRepository.SaveChangesAsync();

        return true;
    }
}