using ECommerce.Application.DTOs.Order;

namespace ECommerce.Application.DTOs.Customer;

public class CustomerDetailsDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsVip { get; set; }

    public List<OrderDto> Orders { get; set; } = new();
}