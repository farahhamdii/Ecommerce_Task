using ECommerce.Application.DTOs.Order;

namespace ECommerce.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderDetailsDto?> GetByIdAsync(int id);

    Task<List<OrderDto>> GetCustomerOrdersAsync(
        int customerId);

    Task<CheckoutResultDto> CheckoutAsync(
        CreateOrderDto dto);

    Task<bool> CancelAsync(int id);
}