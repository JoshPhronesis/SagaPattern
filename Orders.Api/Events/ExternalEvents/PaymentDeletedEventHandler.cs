using Ardalis.GuardClauses;
using OrdersService.Commands;
using SagaPattern.Commons;

namespace OrdersService.Events.ExternalEvents;

public class PaymentDeletedEventHandler : IEventHandler<PaymentCancelledEvent>
{
    private readonly ICommandHandler<DeleteOrderCommand> _commandHandler;

    public PaymentDeletedEventHandler(ICommandHandler<DeleteOrderCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task HandleAsync(PaymentCancelledEvent @event)
    {
        Guard.Against.Null(@event);

        await _commandHandler.HandleAsync(new DeleteOrderCommand(@event.OrderId));
    }
}