namespace ECommerce.Application.DTOs.Customer;

public class CustomerDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsVip { get; set; }
}