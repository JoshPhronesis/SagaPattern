using Payments.Api.Commands;
using SagaPattern.Commons;

namespace Payments.Api.Events.ExternalEvents;

public class ShippingCancelledEventHandler : IEventHandler<ShippingCancelledEvent>
{
    private readonly ICommandHandler<DeletePaymentCommand> _deletePaymentCommandHandler;
    private readonly ISqsMessenger _sqsMessenger;

    public ShippingCancelledEventHandler(ICommandHandler<DeletePaymentCommand> deletePaymentCommandHandler, ISqsMessenger sqsMessenger)
    {
        _deletePaymentCommandHandler = deletePaymentCommandHandler;
        _sqsMessenger = sqsMessenger;
    }
    public async Task HandleAsync(ShippingCancelledEvent @event)
    {
        await _deletePaymentCommandHandler.HandleAsync(new DeletePaymentCommand(@event.PaymentDetailsId));
        
        await _sqsMessenger.SendMessageAsync(new PaymentCancelledEvent
        {
            OrderId = @event.OrderId,
            PaymentDetailsId = @event.PaymentDetailsId
        });
    }
}