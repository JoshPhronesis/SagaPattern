namespace Payments.Api.Events;

public class PaymentCancelledEvent
{
    public Guid OrderId { get; set; }
    public Guid PaymentDetailsId { get; set; }
}