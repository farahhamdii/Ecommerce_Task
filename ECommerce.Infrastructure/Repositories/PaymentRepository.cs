using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;

namespace ECommerce.Infrastructure.Repositories;

public class PaymentRepository
    : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context)
        : base(context)
    {
    }
}