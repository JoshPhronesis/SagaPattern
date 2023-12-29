using SagaPattern.Commons;

namespace Payments.Api.Events.ExternalEvents;

public class OrderCreatedEvent : IEvent
{
    public string Currency { get; set; }
    public double Amount { get; set; }
    public Guid OrderId { get; set; }
}