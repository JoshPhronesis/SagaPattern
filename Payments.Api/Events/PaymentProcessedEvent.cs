namespace Payments.Api.Events;

public class PaymentProcessedEvent
{
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }

    public PaymentProcessedEvent(Guid paymentId, Guid orderId)
    {
        PaymentId = paymentId;
        OrderId = orderId;
    }
}