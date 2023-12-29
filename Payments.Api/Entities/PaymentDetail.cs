using Payments.Api.Enums;

namespace Payments.Api.Entities;

public class PaymentDetail
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
    public PaymentStatus Status { get; set; }
}