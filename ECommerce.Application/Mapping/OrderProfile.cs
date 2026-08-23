using AutoMapper;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Payment;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();

        CreateMap<Order, OrderDetailsDto>();

        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<Payment, PaymentDto>();
    }
}