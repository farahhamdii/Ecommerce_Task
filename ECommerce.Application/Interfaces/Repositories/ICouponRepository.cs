using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface ICouponRepository
    : IGenericRepository<Coupon>
{
    Task<Coupon?> GetActiveByCodeAsync(string code);
}