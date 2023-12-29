using SagaPattern.Commons;

namespace OrdersService.Events;

public class OrderCreatedEvent : IEvent
{
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public Guid OrderId { get; set; }
}