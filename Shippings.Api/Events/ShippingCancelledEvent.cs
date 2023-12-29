using SagaPattern.Commons;

namespace Shipping.Api.Events;

public class ShippingCancelledEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid PaymentDetailsId { get; set; }
}