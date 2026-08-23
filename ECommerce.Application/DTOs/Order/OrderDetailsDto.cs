using ECommerce.Application.DTOs.Payment;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Order;

public class OrderDetailsDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal TotalAmount { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();

    public PaymentDto? Payment { get; set; }
}