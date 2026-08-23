namespace ECommerce.Application.DTOs.Customer;

public class CreateCustomerDto
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsVip { get; set; }
}