using Ardalis.GuardClauses;
using SagaPattern.Commons;
using Shipping.Api.Commands;
using Shipping.Api.Entities;
using Shipping.Api.Enums;

namespace Shipping.Api.Events.ExternalEvents;

public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
{
    private readonly ICommandHandler<CreateShippingRequestCommand> _commandHandler;

    public PaymentProcessedEventHandler(ICommandHandler<CreateShippingRequestCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task HandleAsync(PaymentProcessedEvent @event)
    {
        Guard.Against.Null(@event);
        await _commandHandler.HandleAsync(new(new ShippingRequest
            {
                PaymentId = @event.PaymentId,
                OrderId = @event.OrderId,
                ShippingRequestStatus = ShippingRequestStatus.Open
            })
        );
    }
}