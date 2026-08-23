using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IOrderRepository
    : IGenericRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(int id);

    Task<List<Order>> GetByCustomerIdAsync(
        int customerId);
}