using SagaPattern.Commons;

namespace Shipping.Api.Events.ExternalEvents;

public class PaymentProcessedEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
}