using Payments.Api.Commands;
using Payments.Api.Entities;
using Payments.Api.Enums;
using SagaPattern.Commons;

namespace Payments.Api.Events.ExternalEvents;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ICommandHandler<ProcessPaymentCommand> _handler;

    public OrderCreatedEventHandler(ICommandHandler<ProcessPaymentCommand> handler)
    {
        _handler = handler;
    }
    
    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        await _handler.HandleAsync(new ProcessPaymentCommand(new PaymentDetail
        {
            OrderId = @event.OrderId,
            Amount = @event.Amount,
            Currency = @event.Currency,
            Status = PaymentStatus.UnPaid
        }));
    }
}