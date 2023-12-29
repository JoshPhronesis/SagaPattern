namespace Payments.Api.Dtos;

public class PaymentDetailsForCreateDto
{
    public Guid OrderId { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
}