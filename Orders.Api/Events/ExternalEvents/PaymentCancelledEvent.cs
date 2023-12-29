using SagaPattern.Commons;

namespace OrdersService.Events.ExternalEvents;

public class PaymentCancelledEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid PaymentDetailsId { get; set; }
}