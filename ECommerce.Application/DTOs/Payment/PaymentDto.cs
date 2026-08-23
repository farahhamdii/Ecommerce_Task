namespace ECommerce.Application.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string TransactionReference { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }
}