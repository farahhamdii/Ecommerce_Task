using AutoMapper;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        ICouponRepository couponRepository,
        IPaymentRepository paymentRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _couponRepository = couponRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(int id)
    {
        var order =
            await _orderRepository
                .GetByIdWithDetailsAsync(id);

        if (order == null)
            return null;

        return _mapper.Map<OrderDetailsDto>(order);
    }

    public async Task<List<OrderDto>> GetCustomerOrdersAsync(
        int customerId)
    {
        var orders =
            await _orderRepository
                .GetByCustomerIdAsync(customerId);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<CheckoutResultDto> CheckoutAsync(
        CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            throw new ArgumentException(
                "Cannot checkout an empty order.");
        }

        var customer =
            await _customerRepository
                .GetByIdAsync(dto.CustomerId);

        if (customer == null)
        {
            throw new KeyNotFoundException(
                $"Customer with ID {dto.CustomerId} not found.");
        }

        decimal subtotal = 0m;

        var orderItems = new List<OrderItem>();

        foreach (var itemDto in dto.Items)
        {
            if (itemDto.Quantity <= 0)
            {
                throw new ArgumentException(
                    "Product quantity must be at least 1.");
            }

            var product =
                await _productRepository
                    .GetByIdAsync(itemDto.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID {itemDto.ProductId} not found.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product " +
                    $"'{product.Name}'. " +
                    $"Available: {product.StockQuantity}, " +
                    $"Requested: {itemDto.Quantity}");
            }

            subtotal +=
                product.Price * itemDto.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });

            product.StockQuantity -= itemDto.Quantity;

            _productRepository.Update(product);
        }

        decimal discount = 0m;

        // VIP Discount
        if (customer.IsVip)
        {
            discount +=
                Math.Round(subtotal * 0.15m, 2);
        }

        // Coupon
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var coupon =
                await _couponRepository
                    .GetActiveByCodeAsync(dto.CouponCode);

            if (coupon == null)
            {
                throw new InvalidOperationException(
                    $"Invalid or inactive coupon code " +
                    $"'{dto.CouponCode}'.");
            }

            discount += Math.Round(
                subtotal *
                (coupon.DiscountPercentage / 100m),
                2);
        }

        if (discount > subtotal)
        {
            discount = subtotal;
        }

        var netAmount =
            subtotal - discount;

        var tax =
            Math.Round(netAmount * 0.14m, 2);

        var shipping =
            netAmount >= 1000m
                ? 0m
                : 75m;

        var finalTotal =
            netAmount +
            tax +
            shipping;

        if (finalTotal > 50000m)
        {
            throw new InvalidOperationException(
                "Payment processing failed. " +
                "Amount exceeds limit.");
        }

        var transactionReference =
            $"TX-LEGACY-" +
            Guid.NewGuid()
                .ToString()
                .Substring(0, 8)
                .ToUpper();

        var order = new Order
        {
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Paid,
            Subtotal = subtotal,
            DiscountAmount = discount,
            TaxAmount = tax,
            ShippingFee = shipping,
            TotalAmount = finalTotal,
            Items = orderItems
        };

        var payment = new Payment
        {
            Order = order,
            Amount = finalTotal,
            PaymentDate = DateTime.UtcNow,
            TransactionReference =
                transactionReference,
            IsSuccess = true
        };

        await _orderRepository.AddAsync(order);

        await _paymentRepository.AddAsync(payment);

        await _orderRepository.SaveChangesAsync();

        return new CheckoutResultDto
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            Subtotal = order.Subtotal,
            Discount = order.DiscountAmount,
            Tax = order.TaxAmount,
            Shipping = order.ShippingFee,
            Total = order.TotalAmount,
            TransactionReference =
                transactionReference
        };
    }

    public async Task<bool> CancelAsync(int id)
    {
        var order =
            await _orderRepository
                .GetByIdWithDetailsAsync(id);

        if (order == null)
            return false;

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Order is already cancelled.");
        }

        if (order.Status == OrderStatus.Paid)
        {
            foreach (var item in order.Items)
            {
                var product =
                    await _productRepository
                        .GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    product.StockQuantity +=
                        item.Quantity;

                    _productRepository.Update(product);
                }
            }
        }

        order.Status = OrderStatus.Cancelled;

        _orderRepository.Update(order);

        await _orderRepository.SaveChangesAsync();

        return true;
    }
}