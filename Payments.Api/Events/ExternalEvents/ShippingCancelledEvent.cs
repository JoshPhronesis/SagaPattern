using SagaPattern.Commons;

namespace Payments.Api.Events.ExternalEvents;

public class ShippingCancelledEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid PaymentDetailsId { get; set; }
}