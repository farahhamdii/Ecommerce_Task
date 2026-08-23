using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CouponRepository
    : GenericRepository<Coupon>, ICouponRepository
{
    private readonly AppDbContext _context;

    public CouponRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Coupon?> GetActiveByCodeAsync(
        string code)
    {
        return await _context.Coupons
            .FirstOrDefaultAsync(c =>
                c.Code.ToUpper() == code.ToUpper() &&
                c.IsActive);
    }
}